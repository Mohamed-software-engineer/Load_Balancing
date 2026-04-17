#!/bin/bash

BASE_URL="http://localhost:8090/LoadBalancer/cal?n=1"
METRICS_URL="http://localhost:8090/Metrics"
RESET_URL="http://localhost:8090/Metrics/reset"

mkdir -p results

algorithms=("rr" "random" "wrr" "lc" "hash")

for algo in "${algorithms[@]}"
do
  echo "Running test for $algo ..."

  curl -X POST "$RESET_URL"

  ab -n 10000 -c 200 "${BASE_URL}&algo=${algo}" > "results/${algo}-ab.txt"

  curl "$METRICS_URL" > "results/${algo}-metrics.json"

  echo "Finished $algo"
  echo "-------------------------"
done