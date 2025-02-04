
FROM node:16 AS build


WORKDIR /app

COPY package.json ./


RUN npm install --legacy-peer-deps


RUN apt-get update && \
    apt-get install -y wget apt-transport-https software-properties-common


RUN wget https://packages.microsoft.com/config/debian/11/prod.list \
    -O /etc/apt/sources.list.d/microsoft-prod.list && \
    wget https://packages.microsoft.com/keys/microsoft.asc -O /etc/apt/trusted.gpg.d/microsoft.asc && \
    apt-get update


RUN apt-get install -y dotnet-sdk-8.0



COPY . .


RUN npm run build


FROM nginx:alpine


COPY --from=build /app/dist/client /usr/share/nginx/html


COPY nginx.conf /etc/nginx/nginx.conf


EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]