<?php

$instrumentationKey = '';

function initializeTelemetry()
{
	$instrumentationKey = \OC::$server->getConfig()->getSystemValue('azure.instrumentationkey', '');

	if(!empty($instrumentationKey)){
		require_once 'vendor/autoload.php';
		$url = "http" . (($_SERVER['SERVER_PORT'] == 443) ? "s://" : "://") . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI'];
		$telemetryUrlSelf = $_SERVER['PHP_SELF'];
	}
}

function executeTelemetry($telemetryException)
{
	if(empty($instrumentationKey))
		return;

	$timePassed = round((microtime(true) - $_SERVER["REQUEST_TIME_FLOAT"])*1000);
	$telemetryClient = new \ApplicationInsights\Telemetry_Client();
	$telemetryClient->getContext()->setInstrumentationKey($instrumentationKey);

	if($telemetryException != null) 
	{
		$telemetryClient->trackException($ex);
		$telemetryClient->trackRequest(isset($telemetryUrlSelf) ? $telemetryUrlSelf : '', isset($url) ? $url : '', time(), $timePassed, 500, false);
	} else 
	{
		$telemetryClient->trackRequest(isset($telemetryUrlSelf) ? $telemetryUrlSelf : '', isset($url) ? $url : '', time(), $timePassed, 200, true);
	}
	$telemetryClient->flush();
}