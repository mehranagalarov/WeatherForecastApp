## Weather Forecast Microservice
# Overview
The Weather Forecast Microservice is a Dockerized .NET Core microservice that provides weather forecasts based on city, country, and date inputs. It integrates with multiple weather APIs (such as OpenWeatherAPI, WeatherAPI, and Visual Crossing) to retrieve accurate weather data and caches the response to optimize performance and reduce unnecessary API calls. The service is designed following industry best practices and is deployed on Azure App Service using Docker containers.

# Features
API Integration: Fetches weather forecasts from multiple external APIs.
Date-based Forecast: Retrieves weather forecast for a specific date.
Caching: Implements in-memory caching to minimize repeated API calls.
Dockerized: The service is containerized using Docker for easy deployment.
Azure Deployment: Deployed on Azure App Service for scalable and robust cloud hosting.
Technology Stack
Backend: .NET Core 7.0
Containerization: Docker
Deployment: Azure App Service (Linux)
Caching: In-memory cache using ASP.NET Core MemoryCache
Weather APIs:
OpenWeatherAPI
WeatherAPI
Visual Crossing
# Prerequisites
- .NET Core 7.0 SDK
- Docker
- Azure Subscription for deployment
- API Keys from:
-- OpenWeatherAPI
-- WeatherAPI
-- Visual Crossing API
