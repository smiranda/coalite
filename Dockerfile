FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine3.12 AS build
WORKDIR /app
COPY . .
WORKDIR /app/server
RUN dotnet publish -r linux-musl-x64 -c Release -o /app/deploy/release
RUN dotnet publish -r linux-musl-x64 -c Debug -o /app/deploy/debug
RUN cp nlog.config /app/deploy/debug/
RUN cp nlog.config /app/deploy/release/

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine3.12
COPY --from=build /app/deploy ./app

RUN mkdir /app/state
WORKDIR /app/release

EXPOSE 80
ENTRYPOINT ["./coaliter"]
# To use debug, on docker-compose do
# working_dir: /app/debug