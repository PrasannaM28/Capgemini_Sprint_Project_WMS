# WMS DevOps Pipelines

This folder contains the Azure DevOps YAML needed for production-ready CI/CD.

## Files

- `azure-pipelines.yml`: CI pipeline for restore, build, test, and artifact publishing.
- `release-pipeline.yml`: CD pipeline for deployment to Azure App Service and Azure Static Web Apps.

## What the CI pipeline does

1. Restores the API and test projects directly.
2. Installs Angular dependencies with `npm ci`.
3. Builds the API project and Angular application.
4. Runs automated tests for the API test project and Angular unit tests.
5. Publishes deployable artifacts for the API and frontend.

## What the release pipeline does

1. Downloads the API and frontend artifacts from the CI pipeline.
2. Deploys the API artifact to Azure App Service.
3. Deploys the Angular artifact to Azure Static Web Apps.
4. Uses Azure DevOps environments so you can add manual approvals before deployment.

## Azure DevOps setup steps

### 1. Create the Azure Repo

Put this solution into your Azure DevOps repository and keep the same folder structure. Commit the `WMS.DevOps` folder as part of the repo.

### 2. Create the build pipeline

1. In Azure DevOps, go to `Pipelines` and create a new pipeline from YAML.
2. Point it to `WMS.DevOps/azure-pipelines.yml`.
3. Name the pipeline `WMS-CI` so the release pipeline can reference it.
4. Run it once to verify restore, build, test, and artifact publication.

### 3. Configure the release pipeline

1. Create a second YAML pipeline from `WMS.DevOps/release-pipeline.yml`.
2. Keep `trigger: none` because deployment should follow the CI artifact.
3. Update these variables in the YAML or move them into a variable group:
   - `azureSubscriptionServiceConnection`
   - `apiAppServiceName`
   - `staticWebAppDeploymentToken`
4. Make sure the CI pipeline name in `resources.pipelines.source` matches the build pipeline name.

### 4. Add deployment approvals

Create the following Azure DevOps environments and set approvals on them:

- `wms-production-api`
- `wms-production-web`

Add the required approvers in each environment. The deployment jobs in the YAML will pause until approval is granted.

### 5. Create Azure resources

1. Create an Azure App Service for the API.
2. Create an Azure Static Web App for the Angular frontend.
3. Create an Azure service connection in Azure DevOps that has permission to deploy to the App Service.
4. Copy the Static Web Apps deployment token into a secure variable or variable group.

### 6. Run and validate

1. Commit the YAML files.
2. Queue the CI pipeline manually once.
3. Confirm the API artifact and frontend artifact are published.
4. Run the release pipeline and verify both deployments complete after approval.

## Notes

- The Angular production artifact path is set to `WMS.Frontend/dist/wms-frontend` because that matches the current project name.
- If you later change the Angular project name or output folder, update `frontendDistPath` in `azure-pipelines.yml`.
- The release pipeline is written to use environment approvals instead of hard-coding approval logic in YAML. That is the recommended Azure DevOps approach.