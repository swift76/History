DROP TABLE IF EXISTS book_history;

CREATE TABLE book_history (
    id                  INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    book_id             INTEGER NOT NULL,
    operation_date      TIMESTAMP NOT NULL,
    operation_id        SMALLINT NOT NULL,
    title               TEXT,
    short_description   TEXT,
    publish_date        DATE,
    authors             TEXT[],

    CONSTRAINT fk_book_history_book
        FOREIGN KEY (book_id) REFERENCES book(id)
);

CREATE INDEX i_book_history_1 ON book_history (book_id);

CREATE INDEX i_book_history_2 ON book_history (operation_date);

CREATE INDEX i_book_history_3 ON book_history (operation_id);
