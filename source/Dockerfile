# Build backend
FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app
# Copy csproj and restore as distinct layers
COPY *.sln */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
RUN dotnet restore
# Copy everything else and build
COPY . ./
RUN rm -rf ClientApp
RUN dotnet publish -c Release -o /app/out

# build frontend
FROM node:lts-alpine as build-stage-frontend
WORKDIR /app
COPY SheepIt.Api/ClientApp/package*.json ./
RUN npm install
COPY SheepIt.Api/ClientApp/. .
RUN npm run build

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS final
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-stage-frontend /wwwroot ./wwwroot

RUN mkdir -p /data

EXPOSE 80

COPY ./run-script.sh .
RUN chmod +x ./run-script.sh
CMD ["./run-script.sh"]