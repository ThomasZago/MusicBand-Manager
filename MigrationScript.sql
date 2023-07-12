-- Create the Rehearsal table
CREATE TABLE Rehearsal (
    id INTEGER PRIMARY KEY,
    date DATE,
    notes TEXT,
    worked_songs TEXT,
    setlist_changes TEXT
);

-- Create the Event table
CREATE TABLE Event (
    id INTEGER PRIMARY KEY,
    date DATE,
    setlist_id INTEGER,
    FOREIGN KEY (setlist_id) REFERENCES Setlists(id)
);

-- Create the Member table
CREATE TABLE Member (
    id INTEGER PRIMARY KEY,
    name TEXT,
    arrival_date DATE,
    departure_date DATE
);

-- Create the Repertoire table
CREATE TABLE Repertoire (
    id INTEGER PRIMARY KEY,
    title TEXT,
    style TEXT,
    original_composer TEXT
);

-- Create the Lyrics table
CREATE TABLE Lyrics (
    id INTEGER PRIMARY KEY,
    repertoire_id INTEGER,
    lyrics TEXT,
    FOREIGN KEY (repertoire_id) REFERENCES Repertoire(id)
);

-- Create the InstrumentProgression table
CREATE TABLE InstrumentProgression (
    id INTEGER PRIMARY KEY,
    repertoire_id INTEGER,
    instrument TEXT,
    progression TEXT,
    notes TEXT,
    member_id INTEGER,
    FOREIGN KEY (repertoire_id) REFERENCES Repertoire(id),
    FOREIGN KEY (member_id) REFERENCES Member(id)
);

-- Create the EquipmentInventory table
CREATE TABLE EquipmentInventory (
    id INTEGER PRIMARY KEY,
    item TEXT,
    brand TEXT,
    model TEXT,
    quantity INTEGER,
    future_purchase BOOLEAN,
    member_id INTEGER,
    FOREIGN KEY (member_id) REFERENCES Member(id)
);

-- Create the Setlists table
CREATE TABLE Setlists (
    id INTEGER PRIMARY KEY,
    title TEXT
);

-- Create the SetlistSongs table
CREATE TABLE SetlistSongs (
    id INTEGER PRIMARY KEY,
    setlist_id INTEGER,
    repertoire_id INTEGER,
    position INTEGER,
    note TEXT,
    FOREIGN KEY (setlist_id) REFERENCES Setlists(id),
    FOREIGN KEY (repertoire_id) REFERENCES Repertoire(id)
);
