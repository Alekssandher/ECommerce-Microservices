-- docker/mysql/init/01-create-databases.sql

-- Criar os bancos de dados necessários para os microserviços
CREATE DATABASE IF NOT EXISTS users_auth_service;
CREATE DATABASE IF NOT EXISTS sales_micro_service;
CREATE DATABASE IF NOT EXISTS stock_micro_service;

-- Conceder permissões ao usuário admin
GRANT ALL PRIVILEGES ON users_auth_service.* TO 'admin'@'%';
GRANT ALL PRIVILEGES ON sales_micro_service.* TO 'admin'@'%';
GRANT ALL PRIVILEGES ON stock_micro_service.* TO 'admin'@'%';

FLUSH PRIVILEGES;