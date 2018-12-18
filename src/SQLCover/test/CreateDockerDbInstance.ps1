docker run -d -p 1433:1433 -e sa_password=Psgsgsfsfs!!!!! -e ACCEPT_EULA=Y --name=SQLCover --memory=10g microsoft/mssql-server-windows-developer:2017-GA
docker inspect SQLCover
