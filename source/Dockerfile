# Build backend
FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app
COPY SheepIt.Api/. .
RUN dotnet restore
RUN dotnet build
RUN dotnet publish -c Release -o /app/out

# build frontend
FROM node:lts-alpine as build-stage-frontend
WORKDIR /app
COPY SheepIt.Api/ClientApp/package*.json ./
RUN npm install
COPY SheepIt.Api/ClientApp/. .
RUN npm run build

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS final
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-stage-frontend /wwwroot ./wwwroot

RUN mkdir -p /data

EXPOSE 80
EXPOSE 443

COPY ./run-script.sh .
RUN chmod +x ./run-script.sh
CMD ["./run-script.sh"]
