#build
FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder
WORKDIR /app 

COPY *csproj .
RUN dotnet restore

COPY . . 
RUN dotnet publish -c Release -o publish 

#image creation
FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app 
EXPOSE 80
COPY --from=builder /app/publish . 

ENTRYPOINT ["dotnet","TimeOffRequestSubmission.dll"]

