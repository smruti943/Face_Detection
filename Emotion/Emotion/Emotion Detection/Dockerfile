
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt-get update && apt-get install -y \
    libopenblas-base \
    libx11-6 \
    libc6-dev \
    libgdiplus
WORKDIR /app
EXPOSE 80
EXPOSE 443

 

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Emotion Detection/Emotion Detection.csproj", "Emotion Detection/"]
RUN dotnet restore "Emotion Detection/Emotion Detection.csproj"
COPY . .
WORKDIR "/src/Emotion Detection"
RUN dotnet build "Emotion Detection.csproj" -c Release -o /app/build

 

FROM build AS publish
RUN dotnet publish "Emotion Detection.csproj" -c Release -o /app/publish

 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Emotion Detection.dll"]

