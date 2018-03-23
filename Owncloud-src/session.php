<?php
//simple counter to test sessions. should increment on each page reload.
session_start();
$count = isset($_SESSION['count']) ? $_SESSION['count'] : 1;

echo '1='.$count;

$_SESSION['count'] = ++$count;