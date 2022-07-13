using namespace System.Linq

$filename = "./SmartTender.Api/SmartTender.Api.csproj"

$xml = New-Object XML
$xml.Load($filename)

$element =  $xml.SelectSingleNode("//Project/PropertyGroup/Version")
$versionText = $element.InnerText.ToString().Trim().Split('.') 
$version = 0,0,0
[int]$out = $null
0,1,2 | ForEach-Object { $version[$_] = ([System.Int32]::TryParse($versionText[$_], [ref] $out)) ? $out : 0 }
$version[2]++
$versionText = [System.String]::Join(".",$version)
$element.InnerText = $versionText
$xml.Save($filename)

dotnet pack ./SmartTender.Api/SmartTender.Api.csproj
# dotnet nuget push ./SmartTender.Api/bin/Debug/SmartTender.Commercial.Api.$versionText.nupkg --skip-duplicate --api-key oy2ptwtvcq7j4skeauhqispcgb6yee3e5oxrv4wh4kvb5m --source https://api.nuget.org/v3/index.json













# $xmlDoc = [xml](Get-Content $fileName);
# $xmlDoc..AppendChild("0.0.4")
# $version = $xmlDoc.Project.PropertyGroup.Version[0].ToString().Trim()
# Write-Host $version
# $xmlDoc.OuterXml
# $xmlDoc.Save($fileName);







# $fileName = “employees.xml”;

# $newXmlEmployee = $xmlDoc.employees.AppendChild($xmlDoc.CreateElement("employee"));
# $newXmlEmployee.SetAttribute("id","111");

# $newXmlNameElement = $newXmlEmployee.AppendChild($xmlDoc.CreateElement("name"));
# $newXmlNameTextNode = $newXmlNameElement.AppendChild($xmlDoc.CreateTextNode("Iain Brighton"));

# $newXmlAgeElement = $newXmlEmployee.AppendChild($xmlDoc.CreateElement("age"));
# $newXmlAgeTextNode = $newXmlAgeElement.AppendChild($xmlDoc.CreateTextNode("37"));

# $xmlDoc.Save($fileName);