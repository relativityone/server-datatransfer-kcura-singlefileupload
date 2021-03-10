folder('SimpleFileUpload-Jobs') {
}

buildMonitorView('SimpleFileUpload-Jobs/Monitor')
{
	description('All nighlty jobs')
	recurse(true)
	jobs {
		regex('(Nightly.*)((release(.*))|(develop))')	
	}
	statusFilter(StatusFilter.ENABLED)
}