#!/bin/bash
set -e

until ./efbundle --connection "$ConnectionStrings__DefaultConnection"; do
    >&2 echo "Postgres is starting up - waiting..."
    sleep 1
done

>&2 echo "Postgres is up - executing command"

exec dotnet BankDemo.Api.dll
