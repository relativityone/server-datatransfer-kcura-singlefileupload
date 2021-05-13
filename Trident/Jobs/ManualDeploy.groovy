@Library('ProjectMayhem@v1') _
  
manualDeploy {
    slackChannel = "sfu-deployment" // Optional. Note: currently slack notifications are only sent on build failure
}