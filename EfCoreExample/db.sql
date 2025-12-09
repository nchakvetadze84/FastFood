-- sql
CREATE TABLE [Items] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(100) NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY ([Id])
);

INSERT INTO [Items] ([Name]) VALUES
('Pen'),
('Notebook'),
('Pencil'),
('Marker'),
('Eraser');

-- postgre
-- run docker
-- docker run --name my-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=testdb -p 5432:5432 -d postgres
-- create table + data
-- docker exec -it my-postgres psql -U postgres -d testdb
-- paste following:

CREATE TABLE "Items" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100)
);

INSERT INTO "Items" ("Name") VALUES ('Pen'), ('Notebook'), ('Pencil'), ('Marker'), ('Eraser');

