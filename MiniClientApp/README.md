### About

The project demonstrates working with the MinIO client file storage. 
The examples are taken from the official documentation, with some additional 
modifications that are not included in the examples provided for developers. 
Docker containers were used for testing.

## How to start Docker

To run the test container in Docker, you need to enter the following command in the terminal:
```
docker run -p 9000:9000 -p 9001:9001 --name minio -e "MINIO_ROOT_USER=admin" -e "MINIO_ROOT_PASSWORD=strongpassword" -v /data:/data -v /config:/root/.minio quay.io/minio/minio server /data --console-address ":9001"
```
-p 9000:9000: Opens port 9000 for access to the S3 API.
-p 9001:9001: Opens port 9001 for the MinIO web console.
--name minio: Sets the name of the container.
-e "MINIO_ROOT_USER=admin": Sets the root user (you can replace "admin" with your own).
-e "MINIO_ROOT_PASSWORD=strongpassword": Sets the root password (replace "strongpassword" with a more secure one).
-v /data:/data: Mounts the local /data directory for data storage.
-v /config:/root/.minio: Mounts the directory for configuration files.
