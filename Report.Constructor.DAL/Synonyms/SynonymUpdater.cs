using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Report.Constructor.DAL.Synonyms;

internal static class SynonymUpdater
{
    public static async Task UpdateSynonyms(this DbContext dbContext, IEnumerable<Synonym> synonyms)
    {
        var addSynonymsStringBuilder = new StringBuilder();

        addSynonymsStringBuilder.AppendLine("BEGIN TRANSACTION");
        
        foreach (var synonym in synonyms)
        {
            addSynonymsStringBuilder.AddSynonym(synonym);
        }
        
        addSynonymsStringBuilder.AppendLine("COMMIT TRANSACTION");

        var addSynonymsSql = addSynonymsStringBuilder.ToString();
        
        await dbContext.Database.ExecuteSqlRawAsync(addSynonymsSql);
    }

    private static void AddSynonym(this StringBuilder sb, Synonym synonym)
    {
        sb.AppendLine($$"""
            IF OBJECT_ID('{{synonym.Name}}', 'SN') IS NOT NULL
                DROP SYNONYM {{synonym.Name}};

            CREATE SYNONYM {{synonym.Name}} FOR {{synonym.Value}};
        """);
    }
}