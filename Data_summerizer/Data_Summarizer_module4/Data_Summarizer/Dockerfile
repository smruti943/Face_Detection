#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

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
COPY ["Data_Summarizer/Data_Summarizer.csproj", "Data_Summarizer/"]
RUN dotnet restore "Data_Summarizer/Data_Summarizer.csproj"
COPY . .
WORKDIR "/src/Data_Summarizer"
RUN dotnet build "Data_Summarizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Data_Summarizer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Data_Summarizer.dll"]

