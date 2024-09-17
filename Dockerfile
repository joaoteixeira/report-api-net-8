# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar os arquivos do projeto e restaurar dependências
COPY ./*.csproj ./
RUN dotnet restore

# Copiar o restante do código e compilar a aplicação
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa final (imagem runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

RUN apt-get update && \
    apt-get install -y libgdiplus libc6-dev && \
    apt-get clean && \
    ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
    
WORKDIR /app
COPY --from=build /app/out .


RUN mkdir Reports
COPY Reports/* ./Reports

# Expor a porta que será usada pela API
EXPOSE 80

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "ReportApi8.dll"]
