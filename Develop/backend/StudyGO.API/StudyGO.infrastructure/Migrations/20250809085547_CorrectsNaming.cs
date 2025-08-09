using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyGO.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectsNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorProfilesEntity_FormatsEntity_FormatID",
                table: "TutorProfilesEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorProfilesEntity_UsersEntity_UserID",
                table: "TutorProfilesEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfilesEntity_SubjectsEntity_SubjectID",
                table: "UserProfilesEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfilesEntity_UsersEntity_UserID",
                table: "UserProfilesEntity");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "UsersEntity",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "SubjectID",
                table: "UserProfilesEntity",
                newName: "SubjectId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "UserProfilesEntity",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfilesEntity_SubjectID",
                table: "UserProfilesEntity",
                newName: "IX_UserProfilesEntity_SubjectId");

            migrationBuilder.RenameColumn(
                name: "FormatID",
                table: "TutorProfilesEntity",
                newName: "FormatId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "TutorProfilesEntity",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TutorProfilesEntity_FormatID",
                table: "TutorProfilesEntity",
                newName: "IX_TutorProfilesEntity_FormatId");

            migrationBuilder.RenameColumn(
                name: "SubjectID",
                table: "SubjectsEntity",
                newName: "SubjectId");

            migrationBuilder.RenameColumn(
                name: "FormatID",
                table: "FormatsEntity",
                newName: "FormatId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorProfilesEntity_FormatsEntity_FormatId",
                table: "TutorProfilesEntity",
                column: "FormatId",
                principalTable: "FormatsEntity",
                principalColumn: "FormatId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorProfilesEntity_UsersEntity_UserId",
                table: "TutorProfilesEntity",
                column: "UserId",
                principalTable: "UsersEntity",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfilesEntity_SubjectsEntity_SubjectId",
                table: "UserProfilesEntity",
                column: "SubjectId",
                principalTable: "SubjectsEntity",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfilesEntity_UsersEntity_UserId",
                table: "UserProfilesEntity",
                column: "UserId",
                principalTable: "UsersEntity",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorProfilesEntity_FormatsEntity_FormatId",
                table: "TutorProfilesEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorProfilesEntity_UsersEntity_UserId",
                table: "TutorProfilesEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfilesEntity_SubjectsEntity_SubjectId",
                table: "UserProfilesEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfilesEntity_UsersEntity_UserId",
                table: "UserProfilesEntity");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UsersEntity",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "UserProfilesEntity",
                newName: "SubjectID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserProfilesEntity",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfilesEntity_SubjectId",
                table: "UserProfilesEntity",
                newName: "IX_UserProfilesEntity_SubjectID");

            migrationBuilder.RenameColumn(
                name: "FormatId",
                table: "TutorProfilesEntity",
                newName: "FormatID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TutorProfilesEntity",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_TutorProfilesEntity_FormatId",
                table: "TutorProfilesEntity",
                newName: "IX_TutorProfilesEntity_FormatID");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "SubjectsEntity",
                newName: "SubjectID");

            migrationBuilder.RenameColumn(
                name: "FormatId",
                table: "FormatsEntity",
                newName: "FormatID");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorProfilesEntity_FormatsEntity_FormatID",
                table: "TutorProfilesEntity",
                column: "FormatID",
                principalTable: "FormatsEntity",
                principalColumn: "FormatID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorProfilesEntity_UsersEntity_UserID",
                table: "TutorProfilesEntity",
                column: "UserID",
                principalTable: "UsersEntity",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfilesEntity_SubjectsEntity_SubjectID",
                table: "UserProfilesEntity",
                column: "SubjectID",
                principalTable: "SubjectsEntity",
                principalColumn: "SubjectID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfilesEntity_UsersEntity_UserID",
                table: "UserProfilesEntity",
                column: "UserID",
                principalTable: "UsersEntity",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
