# Load Testing with k6

This directory contains k6 scripts for load testing the UserConnections service.

## Prerequisites

- Install k6: https://k6.io/docs/getting-started/installation/

## Running the Load Test

1. Make sure your UserConnections service is running:
   ```
   dotnet run --project UserConnections.Api
   ```

2. Run the load test:
   ```
   k6 run load-tests/connections-load-test.js
   ```

## Test Configuration

The test is configured to:

- Simulate 50,000 connection requests over 30 seconds
- Ramp up the load from 100 to 3000 requests per second
- Verify that 95% of requests complete successfully
- Verify that 95% of requests complete within 500ms

## Adjusting the Test

You can modify the test parameters in `connections-load-test.js`:

- Change the `startRate` to adjust the initial request rate
- Modify the `stages` to change the ramp-up pattern
- Adjust `preAllocatedVUs` and `maxVUs` based on your system capabilities
- Update the endpoint URL if your service is running on a different port or host

## Viewing Results

After the test completes, k6 will display detailed statistics including:

- Request rates and throughput
- Response times (min, max, median, p90, p95)
- Error rates
- Virtual user metrics

## Alternative Test Parameters

For a different load profile, you can use command-line options:

```
k6 run --vus 1000 --duration 30s load-tests/connections-load-test.js
```

This will run a constant 1000 virtual users for 30 seconds. 