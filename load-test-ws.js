const http = require('http');
const { performance } = require('perf_hooks');

const HOST = 'localhost';
const PORT = 8000;
const CONCURRENCY = 15; // 15 concurrent users
const DURATION_MS = 15000; // 15 seconds duration

const stats = {
  totalRequests: 0,
  successfulRequests: 0,
  failedRequests: 0,
  endpoints: {
    'WS GetPageData': { total: 0, success: 0, failed: 0, latencies: [] },
    'WS UpdatePageContent': { total: 0, success: 0, failed: 0, latencies: [] },
    'HTTP SignIn': { total: 0, success: 0, failed: 0, latencies: [] },
    'HTTP CreateWorkspace': { total: 0, success: 0, failed: 0, latencies: [] },
    'HTTP CreatePage': { total: 0, success: 0, failed: 0, latencies: [] }
  }
};

function recordRequest(endpoint, duration, success = true) {
  stats.totalRequests++;
  if (success) {
    stats.successfulRequests++;
  } else {
    stats.failedRequests++;
  }

  const ep = stats.endpoints[endpoint];
  if (ep) {
    ep.total++;
    if (success) {
      ep.success++;
    } else {
      ep.failed++;
    }
    ep.latencies.push(duration);
  }
}

// Promise wrapper for http.request
function request(options, body = null) {
  return new Promise((resolve, reject) => {
    const start = performance.now();
    const reqOptions = {
      host: HOST,
      port: PORT,
      path: options.path,
      method: options.method || 'GET',
      headers: {
        'Accept': 'application/json',
        ...options.headers
      }
    };

    if (body) {
      reqOptions.headers['Content-Type'] = 'application/json';
    }

    const req = http.request(reqOptions, (res) => {
      let data = '';
      res.on('data', (chunk) => {
        data += chunk;
      });
      res.on('end', () => {
        const duration = performance.now() - start;
        const success = res.statusCode >= 200 && res.statusCode < 300;
        if (options.name) {
          recordRequest(options.name, duration, success);
        }
        resolve({
          statusCode: res.statusCode,
          headers: res.headers,
          body: data
        });
      });
    });

    req.on('error', (err) => {
      const duration = performance.now() - start;
      if (options.name) {
        recordRequest(options.name, duration, false);
      }
      resolve({
        statusCode: 500,
        headers: {},
        body: err.message
      });
    });

    if (body) {
      req.write(typeof body === 'string' ? body : JSON.stringify(body));
    }
    req.end();
  });
}

function extractCookies(headers) {
  const setCookies = headers['set-cookie'];
  if (!setCookies) return '';
  const cookies = [];
  for (const sc of setCookies) {
    const parts = sc.split(';');
    if (parts[0]) {
      cookies.push(parts[0]);
    }
  }
  return cookies.join('; ');
}

const RECORD_SEPARATOR = String.fromCharCode(0x1e);

async function runVirtualUser(vuId, stopSignal) {
  const email = `ws_vu_${vuId}_${Date.now()}@luna.test`;
  let cookieHeader = '';
  let workspaceId = null;
  let pageId = null;

  // 1. Sign In
  try {
    const loginRes = await request({
      path: '/api/v1/Auth/SignIn',
      method: 'POST',
      name: 'HTTP SignIn'
    }, {
      email: email,
      code: 'any'
    });

    if (loginRes.statusCode !== 200) return;
    cookieHeader = extractCookies(loginRes.headers);
  } catch (e) {
    return;
  }

  // 2. Create Workspace
  try {
    const wsRes = await request({
      path: '/api/v1/Workspace/CreateWorkspace',
      method: 'POST',
      headers: { 'Cookie': cookieHeader },
      name: 'HTTP CreateWorkspace'
    }, {
      name: `WS Workspace ${vuId}`,
      icon: '📁',
      defaultPermission: 'view',
      description: 'test'
    });
    if (wsRes.statusCode !== 200) return;
  } catch (e) {
    return;
  }

  // Retrieve Workspace ID
  try {
    const workspacesRes = await request({
      path: '/api/v1/Workspace/GetAvailableWorkspaces',
      method: 'GET',
      headers: { 'Cookie': cookieHeader }
    });
    const list = JSON.parse(workspacesRes.body);
    if (list && list.length > 0) {
      workspaceId = list[0].id || list[0].Id;
    }
  } catch (e) {
    return;
  }

  if (!workspaceId) return;

  // 3. Create Page (with retry loop to wait for Kafka permission sync)
  for (let i = 0; i < 15; i++) {
    try {
      const pageRes = await request({
        path: '/api/v1/Pages/CreatePage',
        method: 'POST',
        headers: { 'Cookie': cookieHeader },
        name: 'HTTP CreatePage'
      }, {
        workspaceId: workspaceId,
        title: `WS Page for VU ${vuId}`,
        emoji: '📄'
      });
      
      if (pageRes.statusCode === 200) {
        pageId = pageRes.body.replace(/"/g, '').trim();
        break;
      }
    } catch (e) {}
    await new Promise(r => setTimeout(r, 200));
  }

  if (!pageId || pageId.length < 10) return;

  // 4. Negotiate SignalR connection
  let connectionToken = '';
  try {
    const negotiateRes = await request({
      path: '/ws/v1/pageHub/negotiate?negotiateVersion=1',
      method: 'POST',
      headers: { 'Cookie': cookieHeader }
    });
    if (negotiateRes.statusCode !== 200) return;
    const negotiateInfo = JSON.parse(negotiateRes.body);
    connectionToken = negotiateInfo.connectionToken;
  } catch (e) {
    return;
  }

  // 5. Connect WebSocket
  return new Promise((resolve) => {
    const wsUrl = `ws://${HOST}:${PORT}/ws/v1/pageHub?id=${connectionToken}`;
    const ws = new WebSocket(wsUrl, {
      headers: { 'Cookie': cookieHeader }
    });

    let isHandshaked = false;
    let nextInvocationId = 1;
    const pendingInvocations = new Map();

    ws.onopen = () => {
      const handshake = JSON.stringify({ protocol: 'json', version: 1 }) + RECORD_SEPARATOR;
      ws.send(handshake);
    };

    ws.onmessage = (event) => {
      const messages = event.data.split(RECORD_SEPARATOR);
      for (const msg of messages) {
        if (!msg) continue;
        const data = JSON.parse(msg);

        // Handshake completion
        if (!isHandshaked && Object.keys(data).length === 0) {
          isHandshaked = true;
          
          // Join the page room
          ws.send(JSON.stringify({
            type: 1,
            invocationId: 'join',
            target: 'JoinPage',
            arguments: [pageId]
          }) + RECORD_SEPARATOR);

          // Start the performance test loop for this VU
          startWorkLoop();
          continue;
        }

        // Completion message (type 3)
        if (data.type === 3) {
          const invId = data.invocationId;
          const pending = pendingInvocations.get(invId);
          if (pending) {
            const duration = performance.now() - pending.start;
            const success = !data.error;
            recordRequest(pending.name, duration, success);
            pendingInvocations.delete(invId);
            pending.resolve();
          }
        }
      }
    };

    ws.onerror = (err) => {
      resolve();
    };

    ws.onclose = () => {
      resolve();
    };

    async function sendInvocation(target, args, name) {
      const invId = (nextInvocationId++).toString();
      const payload = JSON.stringify({
        type: 1,
        invocationId: invId,
        target: target,
        arguments: args
      }) + RECORD_SEPARATOR;

      return new Promise((res) => {
        pendingInvocations.set(invId, {
          start: performance.now(),
          name: name,
          resolve: res
        });
        ws.send(payload);
      });
    }

    async function startWorkLoop() {
      // Loop operations until stop signal is set
      while (!stopSignal.stop) {
        // 1. Get Page Data
        await sendInvocation('GetPageData', [pageId], 'WS GetPageData');

        // 2. Update Page Content
        await sendInvocation('UpdatePageContent', [pageId, {
          document: { type: 'doc', content: [{ type: 'paragraph', text: `Edit from VU ${vuId} at ${Date.now()}` }] },
          changeDescription: `Content revision`
        }], 'WS UpdatePageContent');

        // No delay - fire immediately
        await new Promise(r => setImmediate(r));
      }
      ws.close();
      resolve();
    }
  });
}

async function main() {
  console.log(`========================================`);
  console.log(`Starting WebSocket Load Test against Luna`);
  console.log(`Gateway WebSocket: ws://localhost:8000/ws/v1/pageHub`);
  console.log(`Concurrency (Virtual Users): ${CONCURRENCY}`);
  console.log(`Duration: ${DURATION_MS / 1000} seconds`);
  console.log(`========================================\n`);

  const stopSignal = { stop: false };
  const startTime = performance.now();

  const vuPromises = [];
  for (let i = 1; i <= CONCURRENCY; i++) {
    vuPromises.push(runVirtualUser(i, stopSignal));
  }

  // Run for specified duration
  await new Promise((resolve) => {
    setTimeout(() => {
      stopSignal.stop = true;
      resolve();
    }, DURATION_MS);
  });

  console.log('Stopping virtual users and collecting results...');
  await Promise.all(vuPromises);

  const totalTimeSec = (performance.now() - startTime) / 1000;
  const rps = stats.totalRequests / totalTimeSec;

  console.log(`\n================ TEST SUMMARY ================`);
  console.log(`Total Duration:      ${totalTimeSec.toFixed(2)} seconds`);
  console.log(`Total Requests:      ${stats.totalRequests}`);
  console.log(`Successful Requests: ${stats.successfulRequests}`);
  console.log(`Failed Requests:     ${stats.failedRequests}`);
  console.log(`Overall RPS:         ${rps.toFixed(2)} req/sec`);
  console.log(`==============================================\n`);

  console.log(`Endpoint Performance Breakdown:`);
  console.log(`-------------------------------------------------------------------------------------`);
  console.log(`${'Endpoint'.padEnd(45)} | ${'Total'.padStart(8)} | ${'Success'.padStart(8)} | ${'Failed'.padStart(8)} | ${'Avg Lat (ms)'.padStart(12)}`);
  console.log(`-------------------------------------------------------------------------------------`);

  for (const [name, data] of Object.entries(stats.endpoints)) {
    const avgLat = data.latencies.length > 0 
      ? (data.latencies.reduce((a, b) => a + b, 0) / data.latencies.length).toFixed(1)
      : '0.0';
    console.log(`${name.padEnd(45)} | ${data.total.toString().padStart(8)} | ${data.success.toString().padStart(8)} | ${data.failed.toString().padStart(8)} | ${avgLat.toString().padStart(12)}`);
  }
  console.log(`-------------------------------------------------------------------------------------\n`);
}

main().catch(console.error);
