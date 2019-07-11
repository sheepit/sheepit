#!/bin/bash

echo "Setting up BASE_URL"
sed -i "s@<body><div id=deployment>@<body><script>var BASE_URL = '${BASE_URL}';</script><div id=deployment>@g" wwwroot/index.html
echo "Setting up BASE_URL finished"

echo "Starting Web Server"
cd /app
dotnet SheepIt.Api.dll