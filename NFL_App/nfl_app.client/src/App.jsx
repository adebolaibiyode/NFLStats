import React, { useEffect, useState } from 'react';
import './App.css';


function App() {
  
    const [teams, setTeams] = useState([]); // For dropdown population
    const [teamStats, setTeamStats] = useState([]); // For fetched team stats
    const [selectedTeamId, setSelectedTeamId] = useState('');
    const [gameDate, setGameDate] = useState('');
    const [gameWon, setGameWon] = useState('');
    const [gameLost, setGameLost] = useState('');
    const [teamName, setTeamName] = useState('');

    //Default data load
    useEffect(() => {
        populateNFLTeamData('0');        
        fetchAllTeamsData();
    }, []);

    // Effect for setting gameWon and gameLost based on selectedTeamId
    useEffect(() => {
        const selectedTeam = teams.find(team => team.teamCode === selectedTeamId);
        if (selectedTeam) {
            setGameWon(selectedTeam.gamesWon);
            setGameLost(selectedTeam.gamesLost);
            setTeamName(selectedTeam.teamName);
        } else {
            setGameWon(''); // Reset
            setGameLost(''); // Reset
        }
        // Fetch team stats for the selected team if selectedTeamId changes
        if (selectedTeamId) {
            populateNFLTeamData(selectedTeamId);
        }
    }, [teams, selectedTeamId]); // Dependencies

    const handleInputChange = (e) => {
        setInputValue(e.target.value);
    };

    // Handle form submission
    const handleSubmit = async () => {
        const isValidDate = Date.parse(gameDate);
        if (isValidDate) {
            await fetchTeamStats(selectedTeamId, gameDate);
        } else {
            await populateNFLTeamData(selectedTeamId);
        }
    };

    //populate team data
    async function populateNFLTeamData(teamId) {
        try {
            const response = await fetch(`https://localhost:7025/api/NFLApp/nflteamstat/${teamId}`); 
            const data = await response.ok ? await response.json() : [];
            console.log(data);
            setTeamStats(data);
        }
        catch (error) {
            console.log(error);
            console.error('Failed to fetch team stats:', error);
        }
    }

    // Fetch team stats
    async function fetchTeamStats(teamName, gamedate) {
        try {
            const requestBody = {
                TeamName: teamName,
                Query: gamedate
            };

            const response = await fetch('https://localhost:7025/api/NFLApp/getteamstat', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });

            const data = await response.ok ? await response.json() : null;
            if (data) {
                console.log(data);
                setTeamStats(data);
            } else {
                // Handle the case where no data is returned
                console.log('No data returned for the query');
            }
        } catch (error) {
            console.error('Failed to fetch team stats:', error);
        }
    }

    // Fetch all team data for dropdown
    const fetchAllTeamsData = async () => {
        try
        {
            const response = await fetch(`https://localhost:7025/api/Team/FetchAllTeamsData`);
            const data = await response.ok ? await response.json() : [];
            setTeams(data);
            if (data.length > 0) {
                setSelectedTeamId(data[0].teamCode); // Default to first team                
            }
        } catch (error)
        {
            console.log(error);
        }
    };

    // Handle dropdown change
    const handleDropdownChange = (e) => {
        const newSelectedTeamId = e.target.value;
        setSelectedTeamId(newSelectedTeamId);

        const selectedTeam = teams.find(team => team.teamCode === newSelectedTeamId);
        if (selectedTeam) {
            setGameWon(selectedTeam.gamesWon); 
            setGameLost(selectedTeam.gamesLost); 
            setTeamName(selectedTeam.teamName);

        } else {
            setGameWon(''); // Reset
            setGameLost(''); // Reset
        }

        populateNFLTeamData(newSelectedTeamId); // Fetch team stats for the selected team

    };

    // Display saved data 
    const handleDisplaySavedData = () => {
        console.log(teamStats);
    };

    //Splits game code into meaningful parts for display
    function parseGameCode(gameCode) {
        if (!gameCode || gameCode.length !== 16) return { visitorCode: 'N/A', homeTeamCode: 'N/A', gameDate: 'Invalid Date' };

        const visitorCode = gameCode.substring(0, 4);
        const homeTeamCode = gameCode.substring(4, 8);
        const gameDateStr = gameCode.substring(8);
        const gameDate = `${gameDateStr.substring(0, 4)}-${gameDateStr.substring(4, 6)}-${gameDateStr.substring(6, 8)}`;

        return { visitorCode, homeTeamCode, gameDate };
    }

    // Display team stats content based on the fetched data or a message if no data is available
    const teamStatsContent = teamStats.matchUpStats && teamStats.matchUpStats.length > 0
        ? teamStats.matchUpStats.map(teamStat => {
            // Move the parseGameCode calls outside the JSX return statement
            const homeGameDetails = parseGameCode(teamStat.homeStats.gameCode);
            const visitGameDetails = parseGameCode(teamStat.visStats.gameCode);

            return (
                <div key={teamStat.date} className="team-stat-card">
                    <h2>Team Stats for Team: {teamName}. </h2>
                    {/* Team Stats */}
                    <div className="team-stats-section">
                        <h3>Season Record</h3>
                        <p><b>Total Games Won: </b> {gameWon}</p>
                        <p><b>Total Games Lost: </b> {gameLost}</p>
                        <hr></hr>
                        <h3>Game Date: {new Date(teamStat.date).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })}</h3>
                        <p>Neutral: {String(teamStat.neutral)}</p>
                        <p>Final: {String(teamStat.isFinal)}</p>                       

                    </div>
                    <div className="stats-container">
                        {/* Home Team Stats */}
                        <div className="home-team-stats-section">
                            <h4>Home Team: {teamStat.homeTeamName} - {teamStat.homeStats.teamCode}</h4>
                            <p>First Downs: {teamStat.homeStats.firstDowns || 0}</p>
                            <p>Passing Yards: {teamStat.homeStats.passYds || 0}</p>
                            <p>Rushing Yards: {teamStat.homeStats.rushYds || 0}</p>
                            <p>Penalty Yards: {teamStat.homeStats.penaltYds || 0}</p>
                            <p>Pass Comp: {teamStat.homeStats.passComp || 0}</p>
                            <p>Points: {teamStat.homeStats.score || 0}</p>
                            <p>Penalties: {teamStat.homeStats.penalties || 0}</p>
                            <p>Fumbles Lost: {teamStat.homeStats.fumblesLost || 0}</p>
                            <p>Interceptions Thrown: {teamStat.homeStats.interceptionsThrown || 0}</p>
                            <p>Turnovers (Fumbles and Interceptions): {teamStat.homeStats.fumblesLost + teamStat.homeStats.interceptionsThrown}</p>
                            <p><b>Game Code: {teamStat.homeStats.gameCode || 'N/A'}.</b> <u>See Code Split Below:</u></p>
                            {/* Display parsed game details */}
                            <p><em> ---|| Visitor Code: {homeGameDetails.visitorCode} || Home Team Code: {homeGameDetails.homeTeamCode} || Game Date: {homeGameDetails.gameDate}</em></p>
                        </div>

                        {/* Visiting Team Stats */}
                        <div className="visiting-team-stats-section">
                            <h4>Visiting Team: {teamStat.visTeamName} - {teamStat.visStats.teamCode}</h4>
                            <p>First Downs: {teamStat.visStats.firstDowns || 0}</p>
                            <p>Passing Yards: {teamStat.visStats.passYds || 0}</p>
                            <p>Rushing Yards: {teamStat.visStats.rushYds || 0}</p>
                            <p>Penalty Yards: {teamStat.visStats.penaltYds || 0}</p>
                            <p>Pass Comp: {teamStat.visStats.passComp || 0}</p>
                            <p>Points: {teamStat.visStats.score || 0}</p>
                            <p>Penalties: {teamStat.visStats.penalties || 0}</p>
                            <p>Fumbles Lost: {teamStat.visStats.fumblesLost || 0}</p>
                            <p>Interceptions Thrown: {teamStat.visStats.interceptionsThrown || 0}</p>
                            <p>Turnovers  (Fumbles and Interceptions): {teamStat.visStats.fumblesLost + teamStat.visStats.interceptionsThrown}</p>
                            <p><b>Game Code: {teamStat.visStats.gameCode || 'N/A'}.</b> <u>See Code Split Below:</u> </p>
                            {/* Display parsed game details */}
                            <p><em> ---|| Visitor Code: {visitGameDetails.visitorCode} || Home Team Code: {visitGameDetails.homeTeamCode} || Game Date: {visitGameDetails.gameDate}</em></p>
                        </div>
                    </div>
                </div>
            );
        })
        : <p><em>No Data Available. </em></p>;


    return (
        <div className='team-stat-card'>
            <h1 id="tabelLabel">NFL Team Statistics</h1>
            <div className="team-stats-container">
                <select className='input-box' value={selectedTeamId} onChange={handleDropdownChange}>
                    {teams.map((team) => (
                        <option key={team.teamCode} value={team.teamCode}>
                            {team.teamName}
                        </option>
                    ))}
                </select>
                <button className='button' onClick={handleDisplaySavedData}>Pull NFL Teams</button>
                <hr/>
                <p>Leave Date field blank to load full team stat.</p>
                <input className='input-box'
                    type="date"
                    value={gameDate}
                    onChange={(e) => setGameDate(e.target.value)}
                    placeholder="Select Game Date"
                />
                <hr/>
                <button className='button'  onClick={handleSubmit}>Fetch Team Stats</button>
               
            </div>
            <div className="team-stats-container">
                 {teamStatsContent}
            </div>
        </div>
    );

}

export default App;