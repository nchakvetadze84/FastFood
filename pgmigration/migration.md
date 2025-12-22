# FastFood Migration to PostgreSQL

### Run PostgreDB
```bash
docker run -d --name my-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=FastFood -p 5432:5432 postgres:latest
```

### Install PgLoader
```bash
docker pull dimitri/pgloader
```

### Run PgLoader
```bash
docker run --rm --add-host=host.docker.internal:host-gateway -v %cd%:/data dimitri/pgloader pgloader /data/migration.load
```

### migration.load info
#### BIT/Bool
```load
 CAST type bit to smallint drop typemod;
```
transfer BIT type columns as smallint so 'status = 1' filtering is valid 

#### Quote identifiers
```load
 WITH quote identifiers
```

keeps table/column names case sensetive so its easier to replace column/table names
e.g.
```sql
SELECT [Tables].* FROM [dbo].[Tables]
```
becomes
```sql
SELECT "Tables".* FROM "dbo"."Tables"
```
