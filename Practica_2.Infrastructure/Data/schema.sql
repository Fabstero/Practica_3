-- Habilitar el soporte de llaves foráneas en SQLite (importante ejecutarlo al conectar)
PRAGMA foreign_keys = ON;

-- 1. Tabla de Votantes
CREATE TABLE Votantes (
    IdVotante INTEGER PRIMARY KEY AUTOINCREMENT,
    Cedula TEXT NOT NULL UNIQUE,
    NombreCompleto TEXT NOT NULL,
    FechaNacimiento TEXT NOT NULL, -- Formato recomendado: YYYY-MM-DD
    Provincia TEXT NOT NULL,
    HaVotado INTEGER NOT NULL DEFAULT 0 CHECK (HaVotado IN (0, 1)) -- 0 = No ha votado, 1 = Ya votó
);

-- 2. Tabla de Partidos Políticos
CREATE TABLE PartidosPoliticos (
    IdPartido INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL UNIQUE,
    Siglas TEXT NOT NULL UNIQUE,
    BanderaUrl TEXT NULL, -- Ruta o URL de la imagen/bandera del partido (opcional)
    NombreCandidato TEXT NOT NULL,
    Activo INTEGER NOT NULL DEFAULT 1 CHECK (Activo IN (0, 1)) -- 1 = Activo, 0 = Inactivo
);

-- 3. Tabla de Votos (Registra la transacción de votación)
CREATE TABLE Votos (
    IdVoto INTEGER PRIMARY KEY AUTOINCREMENT,
    IdVotante INTEGER NOT NULL UNIQUE, -- UNIQUE garantiza que un votante solo pueda emitir 1 voto
    IdPartido INTEGER NOT NULL,
    FechaHoraVoto TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (IdVotante) REFERENCES Votantes(IdVotante) ON DELETE RESTRICT,
    FOREIGN KEY (IdPartido) REFERENCES PartidosPoliticos(IdPartido) ON DELETE RESTRICT
);

-- Índices para acelerar las búsquedas por cédula y el conteo de resultados
CREATE INDEX IX_Votantes_Cedula ON Votantes(Cedula);
CREATE INDEX IX_Votos_IdPartido ON Votos(IdPartido);

-- =========================================================================
-- DATOS DE PRUEBA (SEED DATA)
-- =========================================================================

INSERT INTO Votantes (Cedula, NombreCompleto, FechaNacimiento, Provincia, HaVotado)
VALUES
    ('101110111', 'Carlos Rodríguez Mora', '1995-04-12', 'San José', 0),
    ('302220222', 'María Fernanda Solano', '1998-09-23', 'Cartago', 0),
    ('203330333', 'Andrés Jiménez Castro', '1990-01-15', 'Alajuela', 0),
    ('404440444', 'Sofía Vargas Brenes', '2001-11-30', 'Heredia', 0);

INSERT INTO PartidosPoliticos (Nombre, Siglas, NombreCandidato, Activo)
VALUES
    ('Partido Innovación Tecnológica', 'PIT', 'Elena Gómez', 1),
    ('Movimiento Desarrollo Nacional', 'MDN', 'Roberto Alvarado', 1),
    ('Alianza Progreso Verde', 'APV', 'Lucía Méndez', 1);
