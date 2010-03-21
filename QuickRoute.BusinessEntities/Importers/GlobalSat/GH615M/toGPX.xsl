<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:xs="http://www.w3.org/2001/XMLSchema" exclude-result-prefixes="xs"
    
    xmlns:st="urn:uuid:D0EB2ED5-49B6-44e3-B13C-CF15BE7DD7DD" 
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
    xmlns="http://www.topografix.com/GPX/1/1" 
    xsi:schemaLocation="http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd"    
    version="2.0">
    <xsl:output method="xml" indent="yes"/>
    <xd:doc xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 2, 2010</xd:p>
            <xd:p><xd:b>Author:</xd:b> senin</xd:p>
            <xd:p></xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="/">
        <gpx version="1.1" creator="GH-615M Tracks Loader"  >
            <xsl:apply-templates select="track" mode="points"/>
            <xsl:apply-templates select="track" mode="pulse"/>
        </gpx>
    </xsl:template>

    <xsl:template match="track" mode="pulse">
        <extensions>
            <st:activity id="{@id}" startTime="{date}" hasStartTime="true" distanceEntered="{distance}" >
                <xsl:apply-templates select="points" mode="pulse"/>
            </st:activity>            
        </extensions>
    </xsl:template>
    
    <xsl:template match="track" mode="points">
        <trk>
            <name>Track #<xsl:value-of select="@id"/>, <xsl:value-of select="date"/></name>
            <!--<descr>Track #<xsl:value-of select="@id"/>, date: <xsl:value-of select="date"/>, duration: <xsl:value-of select="duration"/> sec, distance: <xsl:value-of select="distance"/> meters</descr>-->
            <xsl:apply-templates select="points" mode="point"/>
        </trk>
    </xsl:template>
    
    <xsl:template match="points" mode="pulse">
        <st:heartRateTrack>
            <xsl:apply-templates select="point" mode="pulse"/>
        </st:heartRateTrack>
    </xsl:template>
    
    <xsl:template match="point" mode="pulse">
        <st:heartRate time="{time}" bpm="{pulse}" />
    </xsl:template>
    
    <xsl:template match="points" mode="point">
        <trkseg>
            <xsl:apply-templates select="point" mode="point"/>
        </trkseg>
    </xsl:template>
    
    <xsl:template match="point" mode="point">
        <trkpt lat="{@lat}" lon="{@lon}" >
            <ele><xsl:value-of select="alt"/></ele>
            <speed><xsl:value-of select="speed div 100"/></speed>
            <time><xsl:value-of select="time"/></time>
        </trkpt>
    </xsl:template>
    
</xsl:stylesheet>
