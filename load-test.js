const http = require('http');
const { performance } = require('perf_hooks');

const GATEWAY_URL = 'http://localhost:8000';
const HOST = 'localhost';
const PORT = 8000;

// Configuration
const CONCURRENCY = 15; // Number of concurrent virtual users
const DURATION_MS = 15000; // Duration of load test (15 seconds)

const keepAliveAgent = new http.Agent({
  keepAlive: true,
  maxSockets: 300,
  maxFreeSockets: 100
});

// Statistics tracking
const stats = {
  totalRequests: 0,
  successfulRequests: 0,
  failedRequests: 0,
  endpoints: {}
};

function recordRequest(endpoint, status, duration, success = true) {
  stats.totalRequests++;
  if (success) {
    stats.successfulRequests++;
  } else {
    stats.failedRequests++;
  }

  if (!stats.endpoints[endpoint]) {
    stats.endpoints[endpoint] = {
      total: 0,
      success: 0,
      failed: 0,
      latencies: []
    };
  }
  const ep = stats.endpoints[endpoint];
  ep.total++;
  if (success) {
    ep.success++;
  } else {
    ep.failed++;
  }
  ep.latencies.push(duration);
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
      agent: keepAliveAgent,
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
        recordRequest(options.name || options.path, res.statusCode, duration, success);
        resolve({
          statusCode: res.statusCode,
          headers: res.headers,
          body: data
        });
      });
    });

    req.on('error', (err) => {
      const duration = performance.now() - start;
      recordRequest(options.name || options.path, 500, duration, false);
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

// Virtual User logic
async function runVirtualUser(vuId, stopSignal) {
  const email = `vu_${vuId}_${Date.now()}@luna.test`;
  let cookieHeader = '';
  let workspaceId = null;

  // 1. Sign In / Register
  try {
    const loginRes = await request({
      path: '/api/v1/Auth/SignIn',
      method: 'POST',
      name: 'POST /Auth/SignIn'
    }, {
      email: email,
      code: 'any_code_since_ignored'
    });

    if (loginRes.statusCode !== 200) {
      console.error(`[VU ${vuId}] Login failed with status: ${loginRes.statusCode}. Response: ${loginRes.body}`);
      return;
    }

    cookieHeader = extractCookies(loginRes.headers);
    if (!cookieHeader) {
      console.error(`[VU ${vuId}] Failed to extract cookies from login response`);
      return;
    }
  } catch (err) {
    console.error(`[VU ${vuId}] Login error:`, err);
    return;
  }

  // 2. Create Workspace
  try {
    const wsRes = await request({
      path: '/api/v1/Workspace/CreateWorkspace',
      method: 'POST',
      headers: { 'Cookie': cookieHeader },
      name: 'POST /Workspace/CreateWorkspace'
    }, {
      name: `Workspace for VU ${vuId}`,
      icon: '📁',
      defaultPermission: 'view',
      description: `Description for Workspace ${vuId}`
    });

    if (wsRes.statusCode !== 200) {
      console.error(`[VU ${vuId}] Create workspace failed: ${wsRes.statusCode}`);
      return;
    }
  } catch (err) {
    console.error(`[VU ${vuId}] Create workspace error:`, err);
    return;
  }

  // 3. Get Available Workspaces to obtain the ID
  try {
    const workspacesRes = await request({
      path: '/api/v1/Workspace/GetAvailableWorkspaces',
      method: 'GET',
      headers: { 'Cookie': cookieHeader },
      name: 'GET /Workspace/GetAvailableWorkspaces'
    });

    if (workspacesRes.statusCode === 200) {
      const list = JSON.parse(workspacesRes.body);
      if (list && list.length > 0) {
        workspaceId = list[0].id || list[0].Id;
      }
    }

    if (!workspaceId) {
      console.error(`[VU ${vuId}] Failed to retrieve workspace ID`);
      return;
    }
  } catch (err) {
    console.error(`[VU ${vuId}] Get workspaces error:`, err);
    return;
  }

  // Loop page operations until stopped
  let pageCount = 0;
  while (!stopSignal.stop) {
    pageCount++;
    let pageId = null;

    // 4. Create Page
    try {
      const createPageRes = await request({
        path: '/api/v1/Pages/CreatePage',
        method: 'POST',
        headers: { 'Cookie': cookieHeader },
        name: 'POST /Pages/CreatePage'
      }, {
        workspaceId: workspaceId,
        title: `Page ${pageCount} for VU ${vuId}`,
        emoji: '📝'
      });

      if (createPageRes.statusCode === 200) {
        pageId = createPageRes.body.replace(/"/g, '').trim();
      }
    } catch (err) {
      // Ignore and continue
    }

    if (pageId && pageId.length > 10) {
      // 5. Read Page
      try {
        await request({
          path: `/api/v1/Pages/GetLightPage?pageId=${pageId}`,
          method: 'GET',
          headers: { 'Cookie': cookieHeader },
          name: 'GET /Pages/GetLightPage'
        });
      } catch (err) {}

      // 6. Delete Page
      try {
        await request({
          path: `/api/v1/Pages/DeletePage?pageId=${pageId}`,
          method: 'DELETE',
          headers: { 'Cookie': cookieHeader },
          name: 'DELETE /Pages/DeletePage'
        });
      } catch (err) {}
    }

    // No delay - fire immediately
    await new Promise(r => setImmediate(r));
  }
}

async function main() {
  console.log(`========================================`);
  console.log(`Starting Load Test against Luna Gateway`);
  console.log(`Gateway Address: ${GATEWAY_URL}`);
  console.log(`Concurrency (Virtual Users): ${CONCURRENCY}`);
  console.log(`Duration: ${DURATION_MS / 1000} seconds`);
  console.log(`========================================\n`);

  const stopSignal = { stop: false };
  const startTime = performance.now();

  // Start VUs
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
