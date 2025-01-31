### About

This project is intended to demonstrate working with the MinIO client file storage in a local environment for private use. The examples are taken from the official documentation, with some additional modifications that are not included in the examples provided for developers. Docker containers were used for testing.

Note: This project is for demonstration purposes only. For use in production environments, it is recommended to replace passwords and other parameters with more secure ones.

## How to start Docker

To run the test container in Docker, you need to enter the following command in the terminal:
```
docker run -p 9000:9000 -p 9001:9001 --name minio1 -v C:\minio\data:/data -e "MINIO_ROOT_USER=admin" -e "MINIO_ROOT_PASSWORD=admin123" quay.io/minio/minio server /data --console-address ":9001"
```
-p 9000:9000: Maps MinIO's default service port to the host.

-p 9001:9001: Maps MinIO's console port to the host.

--name minio1: Names the container minio1.

-v C:\minio\data:/data: Mounts the C:\minio\data directory from the host system as the /data directory in the container.

-e "MINIO_ROOT_USER=admin": Sets the root username for MinIO to admin.

-e "MINIO_ROOT_PASSWORD=admin123": Sets the root password for MinIO to admin123.

quay.io/minio/minio: Specifies the MinIO Docker image to use.

server /data: Indicates that MinIO will use /data as its storage directory.

--console-address ":9001": Sets the MinIO console to listen on port 9001.
## Enable AMPQ
```
docker run --name minio-server -p 9000:9000 -p 9090:9090 -e MINIO_ROOT_USER="admin" -e MINIO_ROOT_PASSWORD="admin123" -e MINIO_NOTIFY_AMQP_ENABLE_PRIMARY="on" -e MINIO_NOTIFY_AMQP_URL_PRIMARY="amqp://admin:admin@rabbitmq:5672" -e MINIO_NOTIFY_AMQP_EXCHANGE_PRIMARY="minio_events" -e MINIO_NOTIFY_AMQP_EXCHANGE_TYPE_PRIMARY="fanout" -e MINIO_NOTIFY_AMQP_ROUTING_KEY_PRIMARY="minio" -e MINIO_BROWSER_REDIRECT_URL="http://localhost:9090" -v C:\minio:/data minio/minio server /data --console-address ":9090"
```
 


## Important

This project can be modified to suit your own needs for local work with MinIO.
