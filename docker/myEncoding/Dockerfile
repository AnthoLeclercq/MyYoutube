FROM  php:8.0.1-apache

# Installing dependencies ans usefull tools
RUN apt-get update && \
    apt-get install -y -qq git \
    zip unzip \
    wget \
    vim \
    ffmpeg

RUN curl -sS https://getcomposer.org/installer | php -- --install-dir=/usr/local/bin --filename=composer

# Where commmand are exec
WORKDIR /var/www/html/

# Setting up Apache2 conf
COPY conf/apache2/000-default.conf /etc/apache2/sites-available/
COPY conf/php/php.ini /usr/local/etc/php/
RUN echo "file_uploads = On\n" \
         "memory_limit = 500M\n" \
         "upload_max_filesize = 500M\n" \
         "post_max_size = 500M\n" \
         "max_execution_time = 600\n" \
         > /usr/local/etc/php/conf.d/uploads.ini

# Letting Apache2 accessing files anywhere
RUN usermod -u 1000 www-data
RUN usermod -G staff www-data

EXPOSE 80
EXPOSE 800
