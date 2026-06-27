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

function request() {
  return new Promise((resolve) => {
    const start = performance.now();
    const reqOptions = {
      host: HOST,
      port: PORT,
      path: '/',
      method: 'GET',
      agent: keepAliveAgent,
      headers: {
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

async function runVirtualUser(stopSignal) {
  while (!stopSignal.stop) {
    await request();
    await new Promise(r => setImmediate(r));
  }
}

async function main() {
  console.log(`========================================`);
  console.log(`Starting Raw Gateway Load Test`);
  console.log(`Target: http://localhost:8000/`);
  console.log(`Concurrency: ${CONCURRENCY}`);
  console.log(`Duration: ${DURATION_MS / 1000} seconds`);
  console.log(`========================================\n`);

  const stopSignal = { stop: false };
  const startTime = performance.now();

  const vuPromises = [];
  for (let i = 0; i < CONCURRENCY; i++) {
    vuPromises.push(runVirtualUser(stopSignal));
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
