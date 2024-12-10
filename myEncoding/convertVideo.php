<?php

var_dump($_POST);
$uploadedFilePath = $_POST['uploadedFilePath'];
$extension = $_POST['extension'];
$userId = $_POST['userId'];
encode_video_to_mp4($uploadedFilePath, $extension, $userId);

// encode_video_to_mp4('var/www/html/upload/1663342836_40_mySampleVideoSentToDotnet.mp4', 'mp4', 40);

function encode_video_to_mp4($uploadedFilePath, $extension, $userId)
{
    $uuidDir = strtoupper(sha1(substr(str_replace("/var/www/html/upload/", "", $uploadedFilePath), 0, 10))); // sha1 of 10 first char file name

    $uploadedFilePath = '/' . $uploadedFilePath;


    mkdir('/var/www/html/encoded/' . $uuidDir, 0777);

    $destinationMP4File = str_replace(
        $extension,
        'mp4',
        str_replace('upload', 'encoded/' . $uuidDir, $uploadedFilePath)
    );

    $ffmpegCommand =
        'ffmpeg -i ' . $uploadedFilePath . ' ' . $destinationMP4File;

    exec($ffmpegCommand);

    gen_different_resolutions($destinationMP4File, $uuidDir, $userId);
}

function gen_different_resolutions($sourceFilePath, $uuidDir, $userId)
{
    $resolutions = ['1920:1080', '1280:720', '854:480', '640:360', '426:240'];
    // $resolutions = ['426:240'];

    foreach ($resolutions as $resolution) {
        $extension_pos = strrpos($sourceFilePath, '.'); // find position of the last dot, so where the extension starts
        $destinationNewResFile =
            substr($sourceFilePath, 0, $extension_pos) .
            '_' .
            $resolution .
            substr($sourceFilePath, $extension_pos);

        $ffmpegCommand = 'ffmpeg -y -i ' . $sourceFilePath . ' -vf scale=' . $resolution . ' ' . $destinationNewResFile;

        exec($ffmpegCommand);
    }

    send_back_videos($uuidDir, $userId);
}

function send_back_videos($uuidDir, $userId)
{
    $target_url = 'back:7277/user/' . $userId . '/video/encoded';
    // $target_url = 'host.docker.internal:7277/user/' . $userId . '/video/encoded';
    $files = scandir('encoded/' . $uuidDir);
    echo "<br>";
    echo $target_url;
    echo "<br>";
    echo "<br>";
    foreach ($files as $file) {
        if ($file != '.' && $file != '..') {
            $file_name_with_full_path = '/var/www/html/encoded/' . $uuidDir . '/' . $file;

            $curl = curl_init();

            curl_setopt_array($curl, array(
              CURLOPT_URL => $target_url,
              CURLOPT_RETURNTRANSFER => true,
              CURLOPT_ENCODING => '',
              CURLOPT_MAXREDIRS => 10,
              CURLOPT_TIMEOUT => 0,
              CURLOPT_FOLLOWLOCATION => true,
              CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
              CURLOPT_CUSTOMREQUEST => 'POST',
              CURLOPT_POSTFIELDS => array('name' => 'Bonjour','source'=> new CURLFILE($file_name_with_full_path)),
              CURLOPT_HTTPHEADER => array(
                'Content-Type: multipart/form-data',
              ),
            ));
            
            $result = curl_exec($curl);

            if (curl_errno($curl)) {
                echo "<br>";
                echo "CURL ERROR - " . curl_error($curl);
                echo "<br>";
            }
            else {
                $info = curl_getinfo($curl);
                print_r($info);
                echo "<br>";
                echo "<br>";
                echo "<br>";
                echo "Result of curl is : ";

                // echo $curl;
            }

            echo "Response = " . "" . $result . "<br>";

            curl_close($curl);
        }
    }
}
