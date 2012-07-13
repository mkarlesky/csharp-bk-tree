
# Construct .Net base installation path and standardize on Ruby's file path separator
DOTNET_BASE_PATH = File.join( ENV['SYSTEMROOT'], 'Microsoft.Net', 'Framework' ).gsub(/\\/, File::SEPARATOR)

# Find all directories within .Net installation path that begin with 'v' (version paths)
DOTNET_VERSIONS = Dir[DOTNET_BASE_PATH + File::SEPARATOR + '*'].select do |path|
    File.directory?(path) &&
    (path.split(File::SEPARATOR)[-1])[0] == 'v'
end

# Add to %PATH% environment variable the last (i.e. latest) of the directories in our .Net install lookup
ENV['PATH'] += File::PATH_SEPARATOR + DOTNET_VERSIONS[-1].gsub(/\//, '\\')


### Constants
PROJECT          = 'BKTree'
UNIT_TESTS_DIR   = 'Tests'
EXE_RELEASE      = "ExampleApplication/bin/Release/ExampleApplication.exe"
EXE_DEBUG        = "ExampleApplication/bin/Debug/ExampleApplication.exe"
DLL_TESTS        = "#{UNIT_TESTS_DIR}/bin/Release/Tests.dll"
EXT_CS           = '.cs'
GLOB_EXT_CS      = "/**/*#{EXT_CS}"


### File lists & directories
SOURCE_DIRS = Dir['.' + File::SEPARATOR + '*'].select do |path|
    File.directory?(path) &&
    !path.end_with?( 'Tests', 'Tools' )
end

SOURCE_FILES = FileList[]
SOURCE_DIRS.each { |dir| SOURCE_FILES.add(FileList[dir + GLOB_EXT_CS]) }

TEST_FILES = FileList[UNIT_TESTS_DIR + GLOB_EXT_CS]


### File tasks
file EXE_DEBUG => SOURCE_FILES do
    msbuild( "#{PROJECT}/#{PROJECT}.csproj", 'build', 'debug' )
end

file EXE_RELEASE => SOURCE_FILES do
    msbuild_release()
end

file DLL_TESTS => (SOURCE_FILES + TEST_FILES) do
    msbuild( "#{UNIT_TESTS_DIR}/Tests.csproj", 'build', 'release' )
end


### Tasks
namespace :clean do
    desc 'Clean debug output'
    task :debug do
        msbuild( "#{PROJECT}.sln", 'clean', 'debug' )
    end
    
    desc 'Clean release output'
    task :release do
        msbuild( "#{PROJECT}.sln", 'clean', 'release' )
    end
    
    desc 'Clean all'
    task :all => ['clean:debug', 'clean:release']
end

namespace :build do
    desc 'Build executable in debug mode'
    task :debug => [EXE_DEBUG]

    desc 'Build executable in release mode'
    task :release => [EXE_RELEASE]
end

namespace :run do
    desc 'Run debug executable'
    task :debug => ['build:debug'] do
        sh EXE_DEBUG
    end

    desc 'Run release executable'
    task :release => ['build:release'] do
        sh EXE_RELEASE
    end
end

desc 'Run unit tests'
task :tests => DLL_TESTS do
    sh "Tools/xunit/xunit.console.clr4.x86.exe #{DLL_TESTS.gsub(/\\/, File::SEPARATOR)}"
end

task :default => ['clean:all', 'tests']


### Functions
def msbuild_release
    msbuild( "ExampleApplication/ExampleApplication.csproj", 'build', 'release' )
end

def msbuild( project, target, config )
    sh "msbuild /nologo \"#{project}\" /t:\"#{target}\" /p:Configuration=\"#{config}\""
end

