import React, { useEffect, useState } from 'react';
import './App.css';


function App() {
    const [teamStats, setTeamStats] = useState([]);
    const [inputValue, setInputValue] = useState('25');

    //Default data load
    useEffect(() => {
        populateNFLTeamData('25');
        setInputValue('25');
    }, []);

    const handleInputChange = (e) => {
        setInputValue(e.target.value);
    };

    const handleSubmit = async () => {
        await populateNFLTeamData(inputValue);
    };  
    
    async function populateNFLTeamData(teamId) {
        try {
            const response = await fetch(`https://localhost:7025/api/NFLApp/nflteamstat/${teamId}`); // Use template literals to insert the teamId
            const data = await response.ok ? await response.json() : [];
            console.log(data);
            setTeamStats(data);
        }
        catch (error) {
            console.log(error);
            console.error('Failed to fetch team stats:', error);
        }
    }

    const teamStatsContent = teamStats.matchUpStats && teamStats.matchUpStats.length > 0
        ? teamStats.matchUpStats.map(teamStat => (
            <div key={teamStat.date} className="team-stat-card">
                {/* Team Stats */}
                <div className="team-stats-section">
                    <h3>Game Date: {new Date(teamStat.date).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })}</h3>
                    <p>Neutral: {String(teamStat.neutral)}</p>
                    <p>Final: {String(teamStat.isFinal)}</p>
                </div>
                <div className="stats-container">
                {/* Home Team Stats */}
                    <div className="home-team-stats-section">
                    <h4>Home Team: {teamStat.homeTeamName}</h4>                    
                    <p>First Downs: {teamStat.homeStats.firstDowns || 0}</p>
                    <p>Passing Yards: {teamStat.homeStats.passYds || 0}</p>
                    <p>Rushing Yards: {teamStat.homeStats.rushYds || 0}</p>
                    <p>Penalty Yards: {teamStat.homeStats.penaltYds || 0}</p>
                    <p>Pass Comp: {teamStat.homeStats.passComp || 0}</p>
                    <p>Points: {teamStat.homeStats.score || 0}</p>
                    <p>Penalties: {teamStat.homeStats.penalties || 0}</p>
                    <p>Fumbles Lost: {teamStat.homeStats.fumblesLost || 0}</p>
                    <p>Interceptions Thrown: {teamStat.homeStats.interceptionsThrown || 0}</p>
                        <p>Turnovers: {teamStat.homeStats.fumblesLost + teamStat.homeStats.interceptionsThrown}</p>
                        <p>Game Code: {teamStat.homeStats.gameCode || 0}</p>
                </div>

                {/* Visiting Team Stats */}
                <div className="visiting-team-stats-section">
                    <h4>Visiting Team: {teamStat.visTeamName}</h4>
                   
                    <p>First Downs: {teamStat.visStats.firstDowns || 0}</p>
                    <p>Passing Yards: {teamStat.visStats.passYds || 0}</p>
                    <p>Rushing Yards: {teamStat.visStats.rushYds || 0}</p>
                    <p>Penalty Yards: {teamStat.visStats.penaltYds || 0}</p>
                    <p>Pass Comp: {teamStat.visStats.passComp || 0}</p>
                    <p>Points: {teamStat.visStats.score || 0}</p>
                    <p>Penalties: {teamStat.visStats.penalties || 0}</p>
                    <p>Fumbles Lost: {teamStat.visStats.fumblesLost || 0}</p>
                    <p>Interceptions Thrown: {teamStat.visStats.interceptionsThrown || 0}</p>
                        <p>Turnovers: {teamStat.visStats.fumblesLost + teamStat.visStats.interceptionsThrown}</p>
                        <p>Game Code: {teamStat.visStats.gameCode || 0}</p>
                    </div>

                    </div>
            </div>
        ))
        : <p><em>Loading... </em></p>;

  
    return (
        <div className='team-stat-card'>
            <h1 id="tabelLabel">NFL Team Statistics</h1>
            <p>Enter Team ID to fetch Team stats.</p>
            <div className="team-stats-container">
                <input className='input-box'
                    type="text"
                    value={inputValue}
                    onChange={handleInputChange}
                    placeholder="Enter Team ID"
                />
                <button className='button' onClick={handleSubmit}>Fetch Team Stats</button>
            </div>
            <div className="team-stats-container">
                {teamStatsContent}
            </div>
        </div>
    );
       

}

export default App;