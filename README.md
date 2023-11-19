# DaprSubscription

to run the KubeMQ example.

KubeMQ must listen on the port 40000

execute this command in a terminal window on the folder DaprSubscription where the csproj is.

dapr run --app-id trial --app-port 5152 --dapr-http-port 3500 --dapr-grpc-port 50000 --resources-path ./Dapr  dotnet run