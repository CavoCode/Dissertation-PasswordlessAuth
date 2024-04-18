IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [FidoKeys] (
    [CredentialId] nvarchar(450) NOT NULL,
    [UserId] nvarchar(450) NULL,
    [UserHandle] nvarchar(max) NULL,
    [DisplayFriendlyName] nvarchar(max) NULL,
    [AttestationType] int NOT NULL,
    [AuthenticatorId] nvarchar(max) NULL,
    [AuthenticatorIdType] int NULL,
    [Counter] int NOT NULL,
    [KeyType] nvarchar(max) NULL,
    [Algorithm] nvarchar(max) NULL,
    [CredentialAsJson] nvarchar(max) NULL,
    [Created] datetime2 NULL,
    [LastUsed] datetime2 NULL,
    CONSTRAINT [PK_FidoKeys] PRIMARY KEY ([CredentialId])
);
GO

CREATE INDEX [IX_FidoKeys_UserId] ON [FidoKeys] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240418193944_Fido', N'8.0.4');
GO

COMMIT;
GO

