using Ydb.Sdk.Client;
using Ydb.Table;
using Ydb.Table.V1;

namespace Ydb.Sdk.Services.Table;

public class CopyTableItem
{
    public string SourcePath { get; }
    public string DestinationPath { get; }
    public bool OmitIndexes { get; }

    public CopyTableItem(string sourcePath, string destinationPath, bool omitIndexes)
    {
        SourcePath = sourcePath;
        DestinationPath = destinationPath;
        OmitIndexes = omitIndexes;
    }

    public Ydb.Table.CopyTableItem GetProto(TableClient tableClient)
    {
        return new Ydb.Table.CopyTableItem
        {
            SourcePath = tableClient.MakeTablePath(SourcePath),
            DestinationPath = tableClient.MakeTablePath(DestinationPath),
            OmitIndexes = OmitIndexes
        };
    }
}

public class CopyTableSettings : OperationRequestSettings
{
}

public class CopyTablesSettings : OperationRequestSettings
{
}

public class CopyTableResponse : ResponseBase
{
    internal CopyTableResponse(Status status) : base(status)
    {
    }
}

public class CopyTablesResponse : ResponseBase
{
    internal CopyTablesResponse(Status status) : base(status)
    {
    }
}

public partial class TableClient
{
    public async Task<CopyTableResponse> CopyTable(string sourcePath, string destinationPath,
        CopyTableSettings? settings = null)
    {
        settings ??= new CopyTableSettings();
        var request = new CopyTableRequest
        {
            OperationParams = MakeOperationParams(settings),
            SourcePath = MakeTablePath(sourcePath),
            DestinationPath = MakeTablePath(destinationPath)
        };

        try
        {
            var response = await Driver.UnaryCall(
                method: TableService.CopyTableMethod,
                request: request,
                settings: settings);

            var status = UnpackOperation(response.Data.Operation);
            return new CopyTableResponse(status);
        }
        catch (Driver.TransportException e)
        {
            return new CopyTableResponse(e.Status);
        }
    }

    public async Task<CopyTablesResponse> CopyTables(List<CopyTableItem> tableItems,
        CopyTablesSettings? settings = null)
    {
        settings ??= new CopyTablesSettings();
        var request = new CopyTablesRequest
        {
            OperationParams = MakeOperationParams(settings)
        };
        request.Tables.AddRange(tableItems.Select(item => item.GetProto(this)));

        try
        {
            var response = await Driver.UnaryCall(
                method: TableService.CopyTablesMethod,
                request: request,
                settings: settings);

            var status = UnpackOperation(response.Data.Operation);
            return new CopyTablesResponse(status);
        }
        catch (Driver.TransportException e)
        {
            return new CopyTablesResponse(e.Status);
        }
    }
}
