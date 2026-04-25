FROM nginx

COPY API.Gateway/nginx/nginx.local.conf /etc/nginx/nginx.conf

COPY API.Gateway/nginx/id-local.crt /etc/ssl/certs/id-local.eshopping.com.crt
COPY API.Gateway/nginx/id-local.key /etc/ssl/private/id-local.eshopping.com.key