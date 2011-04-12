param($installPath, $toolsPath, $package, $project)
$project.Object.References.Add("System.ServiceModel"); 
$project.Object.References.Add("System.ServiceModel.Activation"); 
$project.Object.References.Add("System.ServiceModel.Web"); 



