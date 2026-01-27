using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAIEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Accuracy = table.Column<double>(type: "float(5)", precision: 5, scale: 2, nullable: false),
                    DatasetSize = table.Column<int>(type: "int", nullable: false),
                    LastTrained = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AIModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrainingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    InitialAccuracy = table.Column<double>(type: "float(5)", precision: 5, scale: 2, nullable: false),
                    FinalAccuracy = table.Column<double>(type: "float(5)", precision: 5, scale: 2, nullable: false),
                    Improvement = table.Column<double>(type: "float(5)", precision: 5, scale: 2, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrainingLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metrics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Epochs = table.Column<int>(type: "int", nullable: false),
                    BatchSize = table.Column<int>(type: "int", nullable: false),
                    LearningRate = table.Column<double>(type: "float(10)", precision: 10, scale: 8, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingHistories_AIModels_AIModelId",
                        column: x => x.AIModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AIModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    ActualDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResultAccuracy = table.Column<double>(type: "float(5)", precision: 5, scale: 2, nullable: true),
                    ResultMetrics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingJobs_AIModels_AIModelId",
                        column: x => x.AIModelId,
                        principalTable: "AIModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_Name",
                table: "AIModels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_Status",
                table: "AIModels",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_Type",
                table: "AIModels",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingHistories_AIModelId",
                table: "TrainingHistories",
                column: "AIModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingHistories_Result",
                table: "TrainingHistories",
                column: "Result");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingHistories_TrainingDate",
                table: "TrainingHistories",
                column: "TrainingDate");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingJobs_AIModelId",
                table: "TrainingJobs",
                column: "AIModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingJobs_Priority",
                table: "TrainingJobs",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingJobs_StartedAt",
                table: "TrainingJobs",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingJobs_Status",
                table: "TrainingJobs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingHistories");

            migrationBuilder.DropTable(
                name: "TrainingJobs");

            migrationBuilder.DropTable(
                name: "AIModels");
        }
    }
}

