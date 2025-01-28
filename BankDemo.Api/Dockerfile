FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

#EF Core tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY *.sln .
COPY BankDemo.Api/*.csproj ./BankDemo.Api/
COPY BankDemo.Application/*.csproj ./BankDemo.Application/
COPY BankDemo.Domain/*.csproj ./BankDemo.Domain/
COPY BankDemo.Infrastructure/*.csproj ./BankDemo.Infrastructure/
COPY BankDemo.SharedKernel/*.csproj ./BankDemo.SharedKernel/

RUN dotnet restore

COPY BankDemo.Api/. ./BankDemo.Api/
COPY BankDemo.Application/. ./BankDemo.Application/
COPY BankDemo.Domain/. ./BankDemo.Domain/
COPY BankDemo.Infrastructure/. ./BankDemo.Infrastructure/
COPY BankDemo.SharedKernel/. ./BankDemo.SharedKernel/

WORKDIR /app/BankDemo.Api
RUN dotnet publish -c Release -o out

#migration bundle
WORKDIR /app
ENV ConnectionStrings__DefaultConnection="Host=postgres;Database=BankDb;Username=postgres;Password=1741"
RUN dotnet ef migrations bundle --project BankDemo.Infrastructure --startup-project BankDemo.Api -o efbundle

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build-env /app/BankDemo.Api/out .
COPY --from=build-env /app/efbundle .

COPY BankDemo.Api/entrypoint.sh .
RUN sed -i 's/\r$//' /app/entrypoint.sh //the unix file format
RUN ls -la /app
RUN chmod +x entrypoint.sh

EXPOSE 80
ENTRYPOINT ["./entrypoint.sh"]
