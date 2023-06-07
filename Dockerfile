FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY src .

RUN dotnet restore && dotnet build -c Release --no-restore
CMD chmod +x TemplateProject.Api
RUN dotnet publish "./TemplateProject.Api/TemplateProject.Api.csproj" -c Release -o /app/publish

ENV PATH="${PATH}:~/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef

RUN dotnet dev-certs https

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/

#ENTRYPOINT ["dotnet", "/app/TemplateProject.Api/TemplateProject.Api.dll"]
ENTRYPOINT ["/app/TemplateProject.Api"]