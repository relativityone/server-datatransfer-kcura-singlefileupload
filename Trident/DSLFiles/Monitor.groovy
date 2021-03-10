folder('DataTransfer-Jobs') {
}

folder('DataTransfer-Jobs/SimpleFileUpload') {
}


buildMonitorView('DataTransfer-Jobs/SimpleFileUpload/Monitor')
{
	description('All nighlty jobs')
	recurse(true)
	jobs {
		regex('(Nightly.*)((release(.*))|(develop))')	
	}
	statusFilter(StatusFilter.ENABLED)
}