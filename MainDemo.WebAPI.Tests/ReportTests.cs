using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraPrinting;
using MainDemo.Module.BusinessObjects;
using MainDemo.WebAPI.TestInfrastructure;
using Xunit;

namespace MainDemo.WebAPI.Tests;

public class ReportTests : BaseWebApiTest {
    string DownloadUrl => "/api/Report/DownloadByName";
    string userName = "John";
    string reportName = "Employee List Report";

    public ReportTests(SharedTestHostHolder fixture) : base(fixture) { }

    [Fact]
    public async System.Threading.Tasks.Task LoadFullReport() {
        await WebApiClient.AuthenticateAsync(userName, "");
        string url = CreateRequestUrl(reportName, null, null, null, ExportTarget.Csv);

        string currentData = DateTime.Now.ToShortDateString();
        string newLine = Environment.NewLine;
        string expectedResult =
            $"Employees,,,,{newLine}" +
            $"{currentData},,,,{newLine}" +
            $"A,Last Name,First Name,Email,Phone{newLine}" +
            $",Aiello,Hewitt,Hewitt_Aiello@example.com,(704) 827-5432{newLine}" +
            $"B,Last Name,First Name,Email,Phone{newLine}" +
            $",Borrmann,Aaron,Aaron_Borrmann@example.com,(760) 156-1374{newLine}" +
            $",Bunch,Abigail,Abigail_Bunch@example.com,(404) 943-6711{newLine}" +
            $",Berntsen,Alberta,Alberta_Berntsen@example.com,(702) 649-5647{newLine}" +
            $",Basinger,Almas,Almas_Basinger@example.com,(425) 335-6622{newLine}" +
            $",Benson,Anita,Anita_Benson@example.com,(713) 863-8137{newLine}" +
            $",Boyd,Anita,Anita_Boyd@example.com,(303) 376-7233{newLine}" +
            $",Brandt,Arthur,Arthur_Brandt@example.com,(704) 522-7625{newLine}" +
            $",Baker,Carolyn,Carolyn_Baker@example.com,(209) 125-4334{newLine}" +
            $",Bevington,Chandler,Chandler_Bevington@example.com,(817) 141-7655{newLine}" +
            $",Bradley,Donald,Donald_Bradley@example.com,(801) 271-4121{newLine}" +
            $",Bing,Francine,Francine_Bing@example.com,(720) 861-7141{newLine}" +
            $",Bunkelman,George,George_Bunkelman@example.com,(360) 186-4982{newLine}" +
            $"C,Last Name,First Name,Email,Phone{newLine}" +
            $",Carter,Andrew,Andrew_Carter@example.com,(708) 799-8985{newLine}" +
            $",Cardle,Anita,Anita_Cardle@example.com,(714) 226-8794{newLine}" +
            $",Chase,Arvil,Arvil_Chase@example.com,(718) 193-6521{newLine}" +
            $",Chapman,Barbara,Barbara_Chapman@example.com,(510) 923-7987{newLine}" +
            $",Chinavare,Barbara,Barbara_Chinavare@example.com,(925) 738-9251{newLine}" +
            $",Cambell,Bruce,Bruce_Cambell@example.com,(417) 166-3268{newLine}" +
            $",Campbell,Dailiah,Dailiah_Campbell@example.com,(702) 552-9269{newLine}" +
            $",Catto,Darlene,Darlene_Catto@example.com,(408) 791-9139{newLine}" +
            $",Crimmins,Dora,Dora_Crimmins@example.com,(860) 826-6458{newLine}" +
            $"D,Last Name,First Name,Email,Phone{newLine}" +
            $",Dusek,Alma,Alma_Dusek@example.com,(415) 322-9112{newLine}" +
            $",Deville,Andrea,Andrea_Deville@example.com,(303) 718-1654{newLine}" +
            $"E,Last Name,First Name,Email,Phone\r\n,Etter,Allison,Allison_Etter@example.com,(703) 826-9719\r\nF,Last Name,First Name,Email,Phone\r\n,Faircloth,Barbara,Barbara_Faircloth@example.com,(724) 247-3834\r\nG,Last Name,First Name,Email,Phone\r\n,Gross,Angela,Angela_Gross@example.com,(253) 371-7165\r\n,Geeter,Tony,Tony_Geeter@example.com,(503) 835-2396\r\nH,Last Name,First Name,Email,Phone\r\n,Hively,Alphonzo,Alphonzo_Hively@example.com,(408) 459-7554\r\n,Hanna,Angelia,Angelia_Hanna@example.com,(509) 169-2345\r\n,Hazel,Anthony,Anthony_Hazel@example.com,(801) 831-7151\r\n,Haneline,Cindy,Cindy_Haneline@example.com,(918) 161-3649\r\nJ,Last Name,First Name,Email,Phone\r\n,Johnson,Alphonso,Alphonso_Johnson@example.com,(816) 767-6243\r\n,Jablonski,Karl,Karl_Jablonski@example.com,(716) 673-5435\r\nK,Last Name,First Name,Email,Phone\r\n,Korszniak,Andrew,Andrew_Korszniak@example.com,(970) 534-8756\r\n,Keck,Edward,Edward_Keck@example.com,(216) 192-9699\r\nL,Last Name,First Name,Email,Phone\r\n,Liu,Calvin,Calvin_Liu@example.com,(559) 628-6997\r\n,Limeira,Janete,Janete_Limeira@example.com,(626) 539-3124\r\nM,Last Name,First Name,Email,Phone\r\n,Melton,Alex,Alex_Melton@example.com,(562) 563-8938\r\n,Martin,Alice,Alice_Martin@example.com,(208) 868-1675\r\n,Mccallum,Angela,Angela_Mccallum@example.com,(860) 722-7357\r\n,Matese,Archie,Archie_Matese@example.com,(253) 782-3416\r\nN,Last Name,First Name,Email,Phone\r\n,Nolan,Alfred,Alfred_Nolan@example.com,(817) 964-3798\r\n,Nilsen,John,john_nilsen@example.com,(559) 224-4648\r\nO,Last Name,First Name,Email,Phone\r\n,Oneil,Beverly,Beverly_Oneil@example.com,(408) 788-8461\r\nR,Last Name,First Name,Email,Phone\r\n,Ryan,Anita,Anita_Ryan@example.com,(720) 971-3927\r\n,Rounds,Anthony,Anthony_Rounds@example.com,(559) 453-3698\r\nS,Last Name,First Name,Email,Phone\r\n,Seaman,Amber,Amber_Seaman@example.com,(254) 859-6914\r\n,Stormo,Amos,Amos_Stormo@example.com,(305) 964-4756\r\n,Stamps,Amy,Amy_Stamps@example.com,(617) 342-3285\r\n,Stender,Charles,Charles_Stender@example.com,(240) 242-4822\r\n,Smodey,Harold,Harold_Smodey@example.com,(785) 963-5491\r\n,Scott,Rachel,Rachel_Scott@example.com,(801) 883-2212\r\nT,Last Name,First Name,Email,Phone\r\n,Teter,Essie,Essie_Teter@example.com,(214) 126-8555\r\n,Tellitson,Mary,Mary_Tellitson@example.com,(206) 177-7473\r\nV,Last Name,First Name,Email,Phone\r\n,Vicars,Annie,Annie_Vicars@example.com,(305) 654-4417\r\nW,Last Name,First Name,Email,Phone\r\n,Walker,Albert,Albert_Walker@example.com,(904) 295-9379\r\n,Walker,Angela,Angela_Walker@example.com,(316) 444-3653\r\n,Waytz,Anthony,Anthony_Waytz@example.com,(704) 272-1178\r\n,Webb,Ernest,Ernest_Webb@example.com,(201) 432-6934\r\n,,,,\r\n";
        await LoadReportAndCompare(userName, url, expectedResult);
    }

    [Fact]
    public async System.Threading.Tasks.Task LoadReportWithCriteria() {
        await WebApiClient.AuthenticateAsync(userName, "");

        string criteria = CriteriaOperator.FromLambda<Employee>(x => x.FirstName == "Aaron" || x.LastName == "Benson").ToString();
        string url = CreateRequestUrl(reportName, criteria, null, null, ExportTarget.Csv);

        string currentData = DateTime.Now.ToShortDateString();
        string newLine = Environment.NewLine;
        string expectedResult =
            $"Employees,,,,{newLine}" +
            $"{currentData},,,,{newLine}" +
            $"B,Last Name,First Name,Email,Phone{newLine}" +
            $",Borrmann,Aaron,Aaron_Borrmann@example.com,(760) 156-1374{newLine}" +
            $",Benson,Anita,Anita_Benson@example.com,(713) 863-8137{newLine}" +
            $",,,,{newLine}";
        await LoadReportAndCompare(userName, url, expectedResult);
    }

    private async System.Threading.Tasks.Task LoadReportAndCompare(string userName, string url, string expectedResult) {
        var response = await WebApiClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        Assert.True(response.IsSuccessStatusCode, $"Request failed for {userName} @ {url} ");

        string loadedReport = await response.Content.ReadAsStringAsync();
        Assert.Equal(expectedResult, loadedReport);
        //Assert.True(expectedResult == loadedReport, $"Incorrect response for {userName} @ {url} ");
    }
    private string CreateRequestUrl(
        string reportName, string criteria = null, string reportParameters = null,
        SortProperty[] sortProperties = null, ExportTarget exportType = ExportTarget.Pdf) {

        string url = $"{DownloadUrl}({reportName})";
        var q = $"fileType={exportType}";

        if(!string.IsNullOrEmpty(criteria)) {
            q += $"&criteria={criteria}";
        }
        if(sortProperties != null && sortProperties.Length > 0) {
            foreach(var sortProperty in sortProperties) {
                q += $"&sortProperty={$"{sortProperty.PropertyName},{sortProperty.Direction}"}";
            }
        }
        if(!string.IsNullOrEmpty(reportParameters)) {
            q += $"&{reportParameters}";
        }

        url += "?" + q;

        return url;
    }

}

