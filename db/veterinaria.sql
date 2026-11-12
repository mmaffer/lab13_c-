-- ============================================================
-- Veterinaria - Script PostgreSQL (estructura + datos de ejemplo)
-- Ejecutar:  psql -U postgres -h localhost -f db/veterinaria.sql
-- Si la base ya existe, borrala antes:  DROP DATABASE veterinaria;
-- ============================================================
\encoding UTF8

CREATE DATABASE veterinaria;
\connect veterinaria

-- Un propietario (1) tiene muchas mascotas (N)
CREATE TABLE propietario (
    id_propietario  integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombres         varchar(80)  NOT NULL,
    apellidos       varchar(80)  NOT NULL,
    documento       varchar(15)  NOT NULL UNIQUE,
    telefono        varchar(20),
    email           varchar(100),
    direccion       varchar(150),
    fecha_registro  timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE mascota (
    id_mascota        integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre            varchar(60)  NOT NULL,
    especie           varchar(30)  NOT NULL,
    raza              varchar(60),
    sexo              char(1)      NOT NULL CHECK (sexo IN ('M', 'H')),
    fecha_nacimiento  date,
    peso_kg           numeric(5,2),
    microchip         varchar(20)  UNIQUE,
    activo            boolean      NOT NULL DEFAULT TRUE,
    id_propietario    integer      NOT NULL
        REFERENCES propietario (id_propietario) ON DELETE RESTRICT
);

CREATE INDEX ix_mascota_id_propietario ON mascota (id_propietario);

-- ---------------- Datos de ejemplo ----------------
INSERT INTO propietario (nombres, apellidos, documento, telefono, email, direccion) VALUES
 ('Carlos',  'Ramirez Soto',  '45871236', '987654321', 'carlos.ramirez@example.com', 'Av. La Molina 1234, Lima'),
 ('Lucia',   'Fernandez Paz', '70123456', '956123789', 'lucia.fernandez@example.com', 'Jr. Los Pinos 456, Surco'),
 ('Miguel',  'Torres Vega',   '41236987', '945874123', 'miguel.torres@example.com',   'Calle Las Flores 78, Miraflores'),
 ('Andrea',  'Quispe Rojas',  '72589631', '912345678', 'andrea.quispe@example.com',   'Av. Brasil 910, Jesus Maria'),
 ('Rosa',    'Mendoza Diaz',  '08456321', '999888777', 'rosa.mendoza@example.com',    'Psje. Santa Rosa 12, Rinconada');

INSERT INTO mascota (nombre, especie, raza, sexo, fecha_nacimiento, peso_kg, microchip, activo, id_propietario) VALUES
 ('Firulais', 'Perro', 'Labrador',        'M', '2021-03-15', 28.50, '985112000000001', TRUE,  1),
 ('Mishi',    'Gato',  'Siames',          'H', '2022-07-02',  4.20, '985112000000002', TRUE,  1),
 ('Rocky',    'Perro', 'Bulldog Frances', 'M', '2020-11-20', 12.80, '985112000000003', TRUE,  2),
 ('Luna',     'Gato',  'Persa',           'H', '2023-01-10',  3.90, '985112000000004', TRUE,  2),
 ('Toby',     'Perro', 'Beagle',          'M', '2019-05-30', 14.10, '985112000000005', TRUE,  3),
 ('Nala',     'Perro', 'Golden Retriever','H', '2022-09-12', 25.00, '985112000000006', TRUE,  3),
 ('Piolin',   'Ave',   'Canario',         'M', '2023-04-01',  0.02, NULL,              TRUE,  4),
 ('Copito',   'Conejo','Enano',           'M', '2022-12-25',  1.80, NULL,              TRUE,  4),
 ('Max',      'Perro', 'Pastor Aleman',   'M', '2018-08-08', 34.00, '985112000000009', FALSE, 1),
 ('Kira',     'Gato',  'Mestizo',         'H', '2021-06-18',  4.50, NULL,              TRUE,  2);
-- El propietario 5 (Rosa) queda sin mascotas a proposito: sirve para probar DELETE 204.
