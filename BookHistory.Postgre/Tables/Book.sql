DROP TABLE IF EXISTS book;

CREATE TABLE book (
	id					INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	title				TEXT	NOT NULL,
	short_description	TEXT	NOT NULL,
	publish_date		DATE	NOT NULL,
    authors             TEXT[]	NOT NULL,
	is_deleted			BOOLEAN	NOT NULL	DEFAULT FALSE,
	revision_number		INTEGER	NOT NULL
);
