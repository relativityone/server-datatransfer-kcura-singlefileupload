@Library('ProjectMayhem@v1') _

jobWithSut {
    slackChannel = "ci-sfu"
    sutTemplate = "aio-whitesedge-latest"
	//relativityBranch = "develop"
    jobScript = "Trident/Scripts/nightly-job.ps1"
    cron = "0 3 * * *"
}