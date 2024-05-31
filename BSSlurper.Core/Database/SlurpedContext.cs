using BSSlurper.Core.BeatSaver.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace BSSlurper.Core.Database
{
    public class SlurpedContext : DbContext
    {
        private string DatabaseFile = "beatsaver-data.db";
        public DbSet<UserDetail> Users { get; set; }
        public DbSet<PlaylistWithMaps> Playlists { get; set; }
        public DbSet<MapDetail> Maps { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<MapVersion> Versions { get; set; }
        public DbSet<MapDifficulty> Difficulties { get; set; }

        public SlurpedContext() : base()
        {

        }

        public SlurpedContext(string databaseFile) : base()
        {
            DatabaseFile = databaseFile;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=\"{DatabaseFile}\"");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // DateTimeOffset to UTC DateTime converter
            var dateTimeConverter = new ValueConverter<DateTimeOffset?, DateTime?>(
                dto => dto.HasValue ? dto.Value.UtcDateTime : null,
                dt => dt.HasValue ? new DateTimeOffset(dt.Value, TimeSpan.Zero) : null
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                            .Property(property.Name)
                            .HasConversion(dateTimeConverter)
                            .HasColumnName($"{property.Name}Utc");
                    }
                }
            }

            // UserDetail
            modelBuilder.Entity<UserDetail>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<UserDetail>()
                .Property(u => u.Id)
                .HasColumnName("UserId");

            // PlaylistWithMaps
            modelBuilder.Entity<PlaylistWithMaps>()
                .HasKey(p => p.PlaylistId);

            // Tag
            modelBuilder.Entity<Tag>()
                .Property<int>("TagId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Tag>()
                .HasKey("TagId");

            // MapDetail
            modelBuilder.Entity<MapDetail>()
                .Property<int>("MapId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<MapDetail>()
                .HasKey("MapId");

            modelBuilder.Entity<MapDetail>()
                .Property(m => m.Id)
                .IsRequired();

            modelBuilder.Entity<MapDetail>()
                .HasIndex(m => m.Id)
                .IsUnique();

            modelBuilder.Entity<MapDetail>()
                .HasOne(m => m.Uploader)
                .WithMany();

            modelBuilder.Entity<MapDetail>()
                .OwnsOne(m => m.Metadata);

            modelBuilder.Entity<MapDetail>()
                .OwnsOne(m => m.Stats);

            modelBuilder.Entity<MapDetail>()
                .HasMany(m => m.Tags)
                .WithMany()
                .UsingEntity(j => j.ToTable("MapTags"));

            modelBuilder.Entity<MapDetail>()
                .HasMany(m => m.Versions)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey("VersionMapId");

            modelBuilder.Entity<MapDetail>()
                .HasMany(m => m.OldVersions)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey("OldVersionMapId");

            // MapVersion
            modelBuilder.Entity<MapVersion>()
                .Property<int>("VersionId")
                .IsRequired()
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<MapVersion>()
                .HasKey("VersionId");

            modelBuilder.Entity<MapVersion>()
                .HasIndex(v => v.Hash)
                .IsUnique();

            modelBuilder.Entity<MapVersion>()
                .HasMany(v => v.Diffs)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // MapDifficulty
            modelBuilder.Entity<MapDifficulty>()
                .Property<int>("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<MapDifficulty>()
                .OwnsOne(d => d.ParitySummary);

            // MapDetailWithOrder
            modelBuilder.Entity<MapDetailWithOrder>()
                .Property<int>("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<MapDetailWithOrder>()
                .ToTable("MapsWithOrder");

            modelBuilder.Entity<MapDetailWithOrder>()
                .HasOne(m => m.Map)
                .WithMany();

            // PlaylistWithMaps
            modelBuilder.Entity<PlaylistWithMaps>()
                .ToTable("Playlists");

            modelBuilder.Entity<PlaylistWithMaps>()
                .HasOne(p => p.Owner)
                .WithMany();

            modelBuilder.Entity<PlaylistWithMaps>()
                .HasMany(p => p.Maps)
                .WithMany()
                .UsingEntity(j => j.ToTable("PlaylistMaps"));

            modelBuilder.Entity<PlaylistWithMaps>()
                .OwnsOne(p => p.Stats);
        }

        /// <summary>
        /// Queries both saved data from the database and unsaved changes (tracked entities) from the local context.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="filter">An filter expression to apply to the query.</param>
        /// <returns>A collection of entities that match the specified filter.</returns>
        private ICollection<T> QueryWithLocal<T>(
            Expression<Func<T, bool>> filter)
            where T : class
        {
            // Load saved data from the database
            Set<T>().Where(filter).Load();

            // Get local (unsaved) entities matching the filter
            return filter != null
                ? Set<T>().Local.AsQueryable().Where(filter).ToList()
                : Set<T>().Local.ToList();
        }

        public async Task<PlaylistWithMaps> GetUpdatedPlaylistAsync(PlaylistPage page)
        {
            return await GetUpdatedPlaylistAsync(new PlaylistWithMaps()
            {
                Maps = page.Maps,
                CreatedAt = page.Playlist.CreatedAt,
                Description = page.Playlist.Description,
                DownloadUrl = page.Playlist.DownloadUrl,
                Name = page.Playlist.Name,
                Owner = page.Playlist.Owner,
                PlaylistId = page.Playlist.PlaylistId,
                PlaylistImage = page.Playlist.PlaylistImage,
                PlaylistImage512 = page.Playlist.PlaylistImage512,
                SongsChangedAt = page.Playlist.SongsChangedAt,
                Stats = page.Playlist.Stats,
                Type = page.Playlist.Type,
                UpdatedAt = page.Playlist.UpdatedAt
            });
        }

        public async Task<PlaylistWithMaps> GetUpdatedPlaylistAsync(PlaylistWithMaps playlist)
        {
            var entry = QueryWithLocal<PlaylistWithMaps>(p => p.PlaylistId == playlist.PlaylistId).FirstOrDefault();
            var entryExists = entry != null;

            if (entry != null)
            {
                await entry.LoadRelated(this);
            }

            if (entry == playlist)
            {
                return entry;
            }

            if (entry == null)
            {
                entry = new PlaylistWithMaps()
                {
                    PlaylistId = playlist.PlaylistId,
                };
            }

            if (!entry.Maps.SequenceEqual(playlist.Maps))
            {
                Set<MapDetailWithOrder>().RemoveRange(entry.Maps);
                entry.Maps.Clear();

                foreach (var map in playlist.Maps)
                {
                    entry.Maps.Add(new MapDetailWithOrder()
                    {
                        Map = await GetUpdatedMapDetailAsync(map.Map),
                        Order = map.Order
                    });
                }
            }

            entry.Name = playlist.Name;
            entry.Description = playlist.Description;
            entry.PlaylistImage = playlist.PlaylistImage;
            entry.PlaylistImage512 = playlist.PlaylistImage512;
            entry.Owner = await GetUpdatedUserDetailAsync(playlist.Owner);
            entry.Stats = playlist.Stats;
            entry.CreatedAt = playlist.CreatedAt;
            entry.UpdatedAt = playlist.UpdatedAt;
            entry.SongsChangedAt = playlist.SongsChangedAt;
            entry.DownloadUrl = playlist.DownloadUrl;
            entry.Type = playlist.Type;

            if (entryExists)
            {
                if (Entry(entry).State != EntityState.Added)
                {
                    Entry(entry).State = EntityState.Modified;
                }
            }
            else
            {
                Playlists.Add(entry);
            }

            //await SaveChangesAsync();

            return entry;
        }

        public async Task<UserDetail?> GetUpdatedUserDetailAsync(UserDetail? user)
        {
            if (user == null)
            {
                return null;
            }

            var entry = QueryWithLocal<UserDetail>(u => u.Id == user.Id).FirstOrDefault();
            var entryExists = entry != null;

            if (entryExists && entry.Equals(user))
            {
                return entry;
            }

            if (entry == null)
            {
                entry = new UserDetail()
                {
                    Id = user.Id
                };
            }

            entry.Name = user.Name;
            entry.Avatar = user.Avatar;
            entry.Type = user.Type;
            entry.Admin = user.Admin;
            entry.Curator = user.Curator;
            entry.SeniorCurator = user.SeniorCurator;
            entry.PlaylistUrl = user.PlaylistUrl;
            entry.Hash = user.Hash;
            entry.CuratorTab = user.CuratorTab;
            entry.VerifiedMapper = user.VerifiedMapper;

            if (entryExists)
            {
                if (Entry(entry).State != EntityState.Added)
                {
                    Entry(entry).State = EntityState.Modified;
                }
            }
            else
            {
                Users.Add(entry);
            }

            //await SaveChangesAsync();

            return entry;
        }


        public async Task<Tag> GetUpdatedTagAsync(string tag)
        {
            return await GetUpdatedTagAsync(new Tag()
            {
                Name = tag,
            });
        }

        public async Task<Tag> GetUpdatedTagAsync(Tag tag)
        {
            var entry = QueryWithLocal<Tag>(t => t.Name == tag.Name).FirstOrDefault();
            var entryExists = entry != null;

            if (entryExists)
            {
                return entry;
            }
            else
            {
                entry = new Tag()
                {
                    Name = tag.Name
                };

                Tags.Add(entry);
                //await SaveChangesAsync();
            }

            return entry;
        }

        public async Task<MapDetail> GetUpdatedMapDetailAsync(MapDetail map)
        {
            var entry = QueryWithLocal<MapDetail>(m => m.Id == map.Id).FirstOrDefault();
            var entryExists = entry != null;

            if (entry != null)
            {
                await entry.LoadRelated(this);
            }

            if (entry == map)
            {
                return entry;
            }

            if (entry == null)
            {
                entry = new MapDetail()
                {
                    Id = map.Id
                };
            }

            entry.Name = map.Name;
            entry.Description = map.Description;
            entry.Uploader = await GetUpdatedUserDetailAsync(map.Uploader);
            entry.Metadata = map.Metadata;
            entry.Stats = map.Stats;
            entry.Uploaded = map.Uploaded;
            entry.Automapper = map.Automapper;
            entry.Ranked = map.Ranked;
            entry.Qualified = map.Qualified;

            if (!entry.Versions.SequenceEqual(map.Versions))
            {
                var newHashes = map.Versions.Select(v => v.Hash).ToArray();
                var oldVersions = entry.Versions.Where(v => !newHashes.Contains(v.Hash)).ToArray();
                var versions = map.Versions.ToList();

                entry.Versions.Clear();

                foreach (var version in versions)
                {
                    entry.Versions.Add(await GetUpdatedMapVersionAsync(version));
                }

                foreach (var version in oldVersions)
                {
                    if (!entry.OldVersions.Any(v => v.Hash == version.Hash))
                    {
                        entry.OldVersions.Add(version);
                    }
                }
            }



            if (!(entry.Tags.OrderBy(t => t.Name).SequenceEqual(map.Tags.OrderBy(t => t.Name))))
            {
                var tags = map.Tags.ToList();

                entry.Tags.Clear();

                foreach (var tag in tags)
                {
                    entry.Tags.Add(await GetUpdatedTagAsync(tag));
                }
            }

            entry.CreatedAt = map.CreatedAt;
            entry.UpdatedAt = map.UpdatedAt;
            entry.LastPublishedAt = map.LastPublishedAt;
            entry.Bookmarked = map.Bookmarked;
            entry.DeclaredAi = map.DeclaredAi;
            entry.BlRanked = map.BlRanked;
            entry.BlQualified = map.BlQualified;

            if (entryExists)
            {
                if (Entry(entry).State != EntityState.Added)
                {
                    Entry(entry).State = EntityState.Modified;
                }
            }
            else
            {
                Maps.Add(entry);
            }

            //await SaveChangesAsync();

            return entry;
        }

        public async Task<MapVersion> GetUpdatedMapVersionAsync(MapVersion version)
        {
            var entry = QueryWithLocal<MapVersion>(v => v.Hash == version.Hash).FirstOrDefault();
            var entryExists = entry != null;

            if (entry != null)
            {
                await entry.LoadRelated(this);
            }

            if (entry == version)
            {
                return entry;
            }

            if (entry == null)
            {
                entry = new MapVersion()
                {
                    Hash = version.Hash,
                };
            }

            entry.State = version.State;
            entry.CreatedAt = version.CreatedAt;
            entry.SageScore = version.SageScore;

            if (!entry.Diffs.SequenceEqual(version.Diffs))
            {
                Difficulties.RemoveRange(entry.Diffs.ToArray());
                entry.Diffs = version.Diffs;
            }

            entry.DownloadUrl = version.DownloadUrl;
            entry.CoverUrl = version.CoverUrl;
            entry.PreviewUrl = version.PreviewUrl;

            if (entryExists)
            {
                if (Entry(entry).State != EntityState.Added)
                {
                    Entry(entry).State = EntityState.Modified;
                }
            }
            else
            {
                Versions.Add(entry);
            }

            //await SaveChangesAsync();

            return entry;
        }
    }
}
