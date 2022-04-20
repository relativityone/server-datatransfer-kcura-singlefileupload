@Library('ProjectMayhem@v1') _

jobWithSut {
    slackChannel = "sfu_trident"
    sutTemplate = "aio-tigerlily-1"
	relativityBranch = "develop"
    jobScript = "Trident/Scripts/nightly-job.ps1"
    cron = "0 3 * * *"
}