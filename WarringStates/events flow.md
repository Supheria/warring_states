```mermaid
sequenceDiagram
    MainForm->>+ArchiveSelector: draw client
    ArchiveSelector-->>-MainForm: set bounds
    ArchiveSelector->>+MainForm: archive select
    MainForm->>-ArchiveSelector: disable Listener
    MainForm->>Settings: draw client
```

