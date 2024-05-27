using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSSlurper.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Admin = table.Column<bool>(type: "INTEGER", nullable: false),
                    Curator = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeniorCurator = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlaylistUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Hash = table.Column<string>(type: "TEXT", nullable: true),
                    CuratorTab = table.Column<bool>(type: "INTEGER", nullable: true),
                    VerifiedMapper = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Maps",
                columns: table => new
                {
                    MapId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    UploaderId = table.Column<long>(type: "INTEGER", nullable: true),
                    Metadata_Bpm = table.Column<double>(type: "REAL", nullable: true),
                    Metadata_Duration = table.Column<long>(type: "INTEGER", nullable: true),
                    Metadata_SongName = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata_SongSubName = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata_SongAuthorName = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata_LevelAuthorName = table.Column<string>(type: "TEXT", nullable: true),
                    Stats_Plays = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_Downloads = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_Upvotes = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_Downvotes = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_Score = table.Column<double>(type: "REAL", nullable: true),
                    UploadedUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Automapper = table.Column<bool>(type: "INTEGER", nullable: false),
                    Ranked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Qualified = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastPublishedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Bookmarked = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeclaredAi = table.Column<string>(type: "TEXT", nullable: true),
                    BlRanked = table.Column<bool>(type: "INTEGER", nullable: false),
                    BlQualified = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.MapId);
                    table.ForeignKey(
                        name: "FK_Maps_Users_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    PlaylistId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    PlaylistImage = table.Column<string>(type: "TEXT", nullable: true),
                    PlaylistImage512 = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerId = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_TotalMaps = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_MapperCount = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_TotalDuration = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_MinNps = table.Column<double>(type: "REAL", nullable: true),
                    Stats_MaxNps = table.Column<double>(type: "REAL", nullable: true),
                    Stats_UpVotes = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_DownVotes = table.Column<long>(type: "INTEGER", nullable: true),
                    Stats_AvgScore = table.Column<double>(type: "REAL", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SongsChangedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DownloadUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.PlaylistId);
                    table.ForeignKey(
                        name: "FK_Playlists_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "MapsWithOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MapId = table.Column<int>(type: "INTEGER", nullable: true),
                    Order = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapsWithOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapsWithOrder_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "MapId");
                });

            migrationBuilder.CreateTable(
                name: "MapTags",
                columns: table => new
                {
                    MapDetailMapId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsTagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapTags", x => new { x.MapDetailMapId, x.TagsTagId });
                    table.ForeignKey(
                        name: "FK_MapTags_Maps_MapDetailMapId",
                        column: x => x.MapDetailMapId,
                        principalTable: "Maps",
                        principalColumn: "MapId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapTags_Tags_TagsTagId",
                        column: x => x.TagsTagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    VersionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hash = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SageScore = table.Column<long>(type: "INTEGER", nullable: true),
                    DownloadUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CoverUrl = table.Column<string>(type: "TEXT", nullable: true),
                    PreviewUrl = table.Column<string>(type: "TEXT", nullable: true),
                    OldVersionMapId = table.Column<int>(type: "INTEGER", nullable: true),
                    VersionMapId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.VersionId);
                    table.ForeignKey(
                        name: "FK_Versions_Maps_OldVersionMapId",
                        column: x => x.OldVersionMapId,
                        principalTable: "Maps",
                        principalColumn: "MapId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Versions_Maps_VersionMapId",
                        column: x => x.VersionMapId,
                        principalTable: "Maps",
                        principalColumn: "MapId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistMaps",
                columns: table => new
                {
                    MapsId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlaylistWithMapsPlaylistId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistMaps", x => new { x.MapsId, x.PlaylistWithMapsPlaylistId });
                    table.ForeignKey(
                        name: "FK_PlaylistMaps_MapsWithOrder_MapsId",
                        column: x => x.MapsId,
                        principalTable: "MapsWithOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistMaps_Playlists_PlaylistWithMapsPlaylistId",
                        column: x => x.PlaylistWithMapsPlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "PlaylistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Difficulties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Njs = table.Column<double>(type: "REAL", nullable: false),
                    Offset = table.Column<double>(type: "REAL", nullable: false),
                    Notes = table.Column<long>(type: "INTEGER", nullable: false),
                    Bombs = table.Column<long>(type: "INTEGER", nullable: false),
                    Obstacles = table.Column<long>(type: "INTEGER", nullable: false),
                    Nps = table.Column<double>(type: "REAL", nullable: false),
                    Length = table.Column<double>(type: "REAL", nullable: false),
                    Characteristic = table.Column<string>(type: "TEXT", nullable: true),
                    Difficulty = table.Column<string>(type: "TEXT", nullable: true),
                    Events = table.Column<long>(type: "INTEGER", nullable: false),
                    Chroma = table.Column<bool>(type: "INTEGER", nullable: false),
                    Me = table.Column<bool>(type: "INTEGER", nullable: false),
                    Ne = table.Column<bool>(type: "INTEGER", nullable: false),
                    Cinema = table.Column<bool>(type: "INTEGER", nullable: false),
                    Seconds = table.Column<double>(type: "REAL", nullable: false),
                    ParitySummary_Errors = table.Column<long>(type: "INTEGER", nullable: true),
                    ParitySummary_Warns = table.Column<long>(type: "INTEGER", nullable: true),
                    ParitySummary_Resets = table.Column<long>(type: "INTEGER", nullable: true),
                    MaxScore = table.Column<long>(type: "INTEGER", nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: true),
                    MapVersionVersionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Difficulties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Difficulties_Versions_MapVersionVersionId",
                        column: x => x.MapVersionVersionId,
                        principalTable: "Versions",
                        principalColumn: "VersionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Difficulties_MapVersionVersionId",
                table: "Difficulties",
                column: "MapVersionVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Maps_Id",
                table: "Maps",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Maps_UploaderId",
                table: "Maps",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_MapsWithOrder_MapId",
                table: "MapsWithOrder",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_MapTags_TagsTagId",
                table: "MapTags",
                column: "TagsTagId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistMaps_PlaylistWithMapsPlaylistId",
                table: "PlaylistMaps",
                column: "PlaylistWithMapsPlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_OwnerId",
                table: "Playlists",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_Hash",
                table: "Versions",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Versions_OldVersionMapId",
                table: "Versions",
                column: "OldVersionMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_VersionMapId",
                table: "Versions",
                column: "VersionMapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Difficulties");

            migrationBuilder.DropTable(
                name: "MapTags");

            migrationBuilder.DropTable(
                name: "PlaylistMaps");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "MapsWithOrder");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Maps");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
