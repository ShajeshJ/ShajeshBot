using BnsApis.Models;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BnsApis.SheetApis
{
    public class ItemSearchApi
    {
        private string _spreadsheetId;

        private readonly string InputRange = "API Endpoint!B2";
        private readonly string OutputRange = "API Endpoint!F2";

        SheetsService _sheetService;

        public ItemSearchApi(SheetsService sheetService)
        {
            _sheetService = sheetService;
            _spreadsheetId = Environment.GetEnvironmentVariable("BNS_SPREADSHEET_ID");
        }

        public async Task<int> GetItemId(string searchKey)
        {
            var inputBody = new ValueRange()
            {
                MajorDimension = "ROWS",
                Range = InputRange,
                Values = new List<IList<object>>()
                {
                    new List<object>() { $"*{searchKey}*" }
                }
            };

            var inputRequest = _sheetService.Spreadsheets.Values.Update(inputBody, _spreadsheetId, InputRange);
            inputRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await inputRequest.ExecuteAsync();

            var outputRequest = _sheetService.Spreadsheets.Values.Get(_spreadsheetId, OutputRange);
            var outputResponse = await outputRequest.ExecuteAsync();

            Int32.TryParse(outputResponse.Values?.FirstOrDefault()?.FirstOrDefault()?.ToString(), out var id);

            return id;
        }
    }
}
