const http = require('http');
const { performance } = require('perf_hooks');

const HOST = 'localhost';
const PORT = 8000;
const CONCURRENCY = 30; // 30 concurrent connections
const DURATION_MS = 15000; // 15 seconds test

const keepAliveAgent = new http.Agent({
  keepAlive: true,
  maxSockets: 300,
  maxFreeSockets: 100
});

const stats = {
  totalRequests: 0,
  successfulRequests: 0,
  failedRequests: 0,
  latencies: []
};

// Simple helper to send a HTTP request
function makeHttpRequest(options, bodyData = null) {
  return new Promise((resolve, reject) => {
    const req = http.request(options, (res) => {
      let data = '';
      res.on('data', chunk => data += chunk);
      res.on('end', () => resolve({ statusCode: res.statusCode, headers: res.headers, data }));
    });
    req.on('error', err => reject(err));
    if (bodyData) {
      req.write(bodyData);
    }
    req.end();
  });
}

async function authenticate() {
  const email = `test-gateway-${Date.now()}@luna.test`;
  const postData = JSON.stringify({
    email: email,
    code: 'any_code_since_ignored'
  });

  const options = {
    host: HOST,
    port: PORT,
    path: '/api/v1/Auth/SignIn',
    method: 'POST',
    agent: keepAliveAgent,
    headers: {
      'Content-Type': 'application/json',
      'Content-Length': Buffer.byteLength(postData)
    }
  };

  console.log(`Authenticating user: ${email}...`);
  const res = await makeHttpRequest(options, postData);
  if (res.statusCode !== 200) {
    throw new Error(`Authentication failed with status ${res.statusCode}: ${res.data}`);
  }

  // Extract cookies
  const setCookieHeaders = res.headers['set-cookie'] || [];
  const cookies = setCookieHeaders.map(cookie => cookie.split(';')[0]).join('; ');
  if (!cookies.includes('access_token')) {
    throw new Error('Authentication succeeded but access_token cookie was not found!');
  }
  console.log('Authentication successful. Cookies acquired.');
  return cookies;
}

function authRequest(cookies) {
  return new Promise((resolve) => {
    const start = performance.now();
    const reqOptions = {
      host: HOST,
      port: PORT,
      path: '/test-auth',
      method: 'GET',
      agent: keepAliveAgent,
      headers: {
        'Cookie': cookies,
        'Accept': 'text/plain'
      }
    };

    const req = http.request(reqOptions, (res) => {
      res.on('data', () => {});
      res.on('end', () => {
        const duration = performance.now() - start;
        const success = res.statusCode === 200;
        stats.totalRequests++;
        if (success) {
          stats.successfulRequests++;
        } else {
          stats.failedRequests++;
        }
        stats.latencies.push(duration);
        resolve();
      });
    });

    req.on('error', () => {
      stats.totalRequests++;
      stats.failedRequests++;
      resolve();
    });

    req.end();
  });
}

async function runVirtualUser(cookies, stopSignal) {
  while (!stopSignal.stop) {
    await authRequest(cookies);
    await new Promise(r => setImmediate(r));
  }
}

async function main() {
  let cookies;
  try {
    cookies = await authenticate();
  } catch (err) {
    console.error('Initial authentication failed:', err.message);
    process.exit(1);
  }

  console.log(`\n========================================`);
  console.log(`Starting Gateway JWT Authentication Load Test`);
  console.log(`Target: http://localhost:8000/test-auth`);
  console.log(`Concurrency: ${CONCURRENCY}`);
  console.log(`Duration: ${DURATION_MS / 1000} seconds`);
  console.log(`========================================\n`);

  const stopSignal = { stop: false };
  const startTime = performance.now();

  const vuPromises = [];
  for (let i = 0; i < CONCURRENCY; i++) {
    vuPromises.push(runVirtualUser(cookies, stopSignal));
  }

  await new Promise((resolve) => {
    setTimeout(() => {
      stopSignal.stop = true;
      resolve();
    }, DURATION_MS);
  });

  await Promise.all(vuPromises);

  const totalTimeSec = (performance.now() - startTime) / 1000;
  const rps = stats.totalRequests / totalTimeSec;
  const avgLat = stats.latencies.length > 0
    ? (stats.latencies.reduce((a, b) => a + b, 0) / stats.latencies.length).toFixed(2)
    : '0.00';

  console.log(`\n================ TEST SUMMARY ================`);
  console.log(`Total Duration:      ${totalTimeSec.toFixed(2)} seconds`);
  console.log(`Total Requests:      ${stats.totalRequests}`);
  console.log(`Successful Requests: ${stats.successfulRequests}`);
  console.log(`Failed Requests:     ${stats.failedRequests}`);
  console.log(`Average Latency:     ${avgLat} ms`);
  console.log(`Overall RPS:         ${rps.toFixed(2)} req/sec`);
  console.log(`==============================================\n`);
}

main().catch(console.error);
